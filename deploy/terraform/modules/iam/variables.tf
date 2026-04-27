

variable "project_name" {
  description = "Project name used for naming IAM resources"
  type        = string
}

variable "environment" {
  description = "Environment name such as dev, staging, or prod"
  type        = string
}

variable "s3_bucket_names" {
  description = "List of existing S3 bucket names that application EC2 instances need to access"
  type        = list(string)
}

variable "allow_s3_delete" {
  description = "Whether to allow DeleteObject permission on the S3 bucket"
  type        = bool
  default     = false
}

variable "common_tags" {
  description = "Common tags applied to IAM resources where supported"
  type        = map(string)
  default     = {}
}