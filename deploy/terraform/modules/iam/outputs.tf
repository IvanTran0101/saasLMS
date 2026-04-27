

output "app_ec2_role_name" {
  description = "Name of the IAM role used by application EC2 instances"
  value       = aws_iam_role.app_ec2_role.name
}

output "app_ec2_role_arn" {
  description = "ARN of the IAM role used by application EC2 instances"
  value       = aws_iam_role.app_ec2_role.arn
}

output "app_instance_profile_name" {
  description = "Name of the IAM instance profile attached to application EC2 instances"
  value       = aws_iam_instance_profile.app_instance_profile.name
}

output "app_instance_profile_arn" {
  description = "ARN of the IAM instance profile attached to application EC2 instances"
  value       = aws_iam_instance_profile.app_instance_profile.arn
}

output "s3_bucket_access_policy_arn" {
  description = "ARN of the IAM policy granting access to the configured S3 buckets"
  value       = aws_iam_policy.s3_bucket_access.arn
}