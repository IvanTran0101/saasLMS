#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TARGET_ENV="${1:-prod}"
TERRAFORM_ENV_DIR="${SCRIPT_DIR}/../environments/${TARGET_ENV}"
ANSIBLE_INV_INI_FILE="${SCRIPT_DIR}/../../ansible/inventories/${TARGET_ENV}/hosts.ini"
ANSIBLE_INV_YML_FILE="${SCRIPT_DIR}/../../ansible/inventories/${TARGET_ENV}/hosts.yml"

if [[ ! -d "${TERRAFORM_ENV_DIR}" ]]; then
  echo "Terraform environment directory not found: ${TERRAFORM_ENV_DIR}" >&2
  exit 1
fi

if [[ ! -d "$(dirname "${ANSIBLE_INV_INI_FILE}")" ]]; then
  echo "Ansible inventory directory not found: $(dirname "${ANSIBLE_INV_INI_FILE}")" >&2
  exit 1
fi

if ! command -v terraform >/dev/null 2>&1; then
  echo "terraform is required but not found in PATH." >&2
  exit 1
fi

if ! command -v jq >/dev/null 2>&1; then
  echo "jq is required but not found in PATH." >&2
  exit 1
fi

cd "${TERRAFORM_ENV_DIR}"

tf_json="$(terraform output -json)"

existing_manager_host=""
existing_infra_host=""
existing_ssh_key_file="~/.ssh/saaslms.pem"
if [[ -f "${ANSIBLE_INV_INI_FILE}" ]]; then
  existing_manager_host="$(awk '/^\[manager\]/{f=1; next} /^\[/{f=0} f && /ansible_host=/{for(i=1;i<=NF;i++) if($i ~ /^ansible_host=/){sub(/^ansible_host=/,"",$i); print $i; exit}}' "${ANSIBLE_INV_INI_FILE}")"
  existing_infra_host="$(awk '/^\[infra\]/{f=1; next} /^\[/{f=0} f && /ansible_host=/{for(i=1;i<=NF;i++) if($i ~ /^ansible_host=/){sub(/^ansible_host=/,"",$i); print $i; exit}}' "${ANSIBLE_INV_INI_FILE}")"
  existing_ssh_key_file="$(awk -F= '/^ansible_ssh_private_key_file=/{print $2; exit}' "${ANSIBLE_INV_INI_FILE}")"
fi

manager_public_ip="$(jq -r '.manager_public_ip.value // empty' <<< "${tf_json}")"
manager_private_ip="$(jq -r '.manager_private_ip.value // empty' <<< "${tf_json}")"
infra_public_ip="$(jq -r '.infra_public_ip.value // empty' <<< "${tf_json}")"
infra_private_ip="$(jq -r '.infra_private_ip.value // empty' <<< "${tf_json}")"

if [[ -z "${manager_public_ip}" ]]; then
  manager_public_ip="${existing_manager_host}"
fi

if [[ -z "${infra_public_ip}" ]]; then
  infra_public_ip="${existing_infra_host:-${infra_private_ip}}"
fi

if [[ -z "${manager_public_ip}" || -z "${manager_private_ip}" || -z "${infra_public_ip}" || -z "${infra_private_ip}" ]]; then
  echo "Missing required terraform outputs. Ensure apply is complete and outputs are available." >&2
  exit 1
fi

cat > "${ANSIBLE_INV_INI_FILE}" <<EOF
[manager]
manager1 ansible_host=${manager_public_ip} manager_private_ip=${manager_private_ip}

[workers]
# Workers are discovered dynamically via workers.aws_ec2.yml

[infra]
infra1 ansible_host=${infra_public_ip} infra_private_ip=${infra_private_ip}

[all:vars]
ansible_user=ubuntu
ansible_ssh_private_key_file=${existing_ssh_key_file}
ansible_python_interpreter=/usr/bin/python3
EOF

cat > "${ANSIBLE_INV_YML_FILE}" <<EOF
all:
  children:
    manager:
      hosts:
        manager1:
          ansible_host: ${manager_public_ip}
          manager_private_ip: ${manager_private_ip}

    workers:
      hosts: {}

    infra:
      hosts:
        infra1:
          ansible_host: ${infra_public_ip}
          infra_private_ip: ${infra_private_ip}

  vars:
    ansible_user: ubuntu
    ansible_ssh_private_key_file: ${existing_ssh_key_file}
    ansible_python_interpreter: /usr/bin/python3
EOF

echo "Rendered ${ANSIBLE_INV_INI_FILE} and ${ANSIBLE_INV_YML_FILE} from terraform outputs."
