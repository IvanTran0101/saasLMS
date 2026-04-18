#!/usr/bin/env sh
set -eu

load_secret() {
  secret_name="$1"
  env_name="$2"

  if [ -f "/run/secrets/$secret_name" ]; then
    value="$(tr -d '\r' < "/run/secrets/$secret_name")"
    export "$env_name=$value"
  fi
}

[ "$#" -gt 0 ] || {
  echo "No startup command provided."
  exit 1
}

load_secret abplicensecode AbpLicenseCode
load_secret stringencryption_defaultpassphrase StringEncryption__DefaultPassPhrase
load_secret identityclients_default_clientsecret IdentityClients__Default__ClientSecret

load_secret connectionstrings_administrationservice ConnectionStrings__AdministrationService
load_secret connectionstrings_assessmentservice ConnectionStrings__AssessmentService
load_secret connectionstrings_coursecatalogservice ConnectionStrings__CourseCatalogService
load_secret connectionstrings_enrollmentservice ConnectionStrings__EnrollmentService
load_secret connectionstrings_forms ConnectionStrings__Forms
load_secret connectionstrings_identityservice ConnectionStrings__IdentityService
load_secret connectionstrings_learningprogressservice ConnectionStrings__LearningProgressService
load_secret connectionstrings_notificationservice ConnectionStrings__NotificationService
load_secret connectionstrings_productservice ConnectionStrings__ProductService
load_secret connectionstrings_reportingservice ConnectionStrings__ReportingService
load_secret connectionstrings_saasservice ConnectionStrings__SaasService

exec "$@"