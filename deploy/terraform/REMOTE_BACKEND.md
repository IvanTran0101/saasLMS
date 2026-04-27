Use an S3 backend with DynamoDB locking for production environments.

Recommended resources per AWS account:

- S3 bucket: `saaslms-terraform-state-<account-id>-ap-southeast-1`
- DynamoDB table: `saaslms-terraform-locks`

Example backend config for `prod-account-a`:

```hcl
bucket         = "saaslms-terraform-state-864946423771-ap-southeast-1"
key            = "prod-account-a/terraform.tfstate"
region         = "ap-southeast-1"
profile        = "account-a"
dynamodb_table = "saaslms-terraform-locks"
encrypt        = true
```

Bootstrap flow:

1. Create the S3 bucket and DynamoDB table once, outside the environment state.
2. Add a `backend "s3" {}` block to the environment.
3. Run:

```bash
terraform init \
  -backend-config=backend.prod-account-a.hcl \
  -migrate-state
```

Notes:

- Keep one backend config file per environment/account.
- Do not rely on local state for production.
- After backend migration, run `terraform plan` again before applying.
