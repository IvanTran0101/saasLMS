output "vpc_id" {
  description = "ID of the VPC created for the production environment"
  value       = module.vpc.vpc_id
}

output "public_subnet_ids" {
  description = "List of public subnet IDs in the production environment"
  value       = module.vpc.public_subnet_ids
}

output "private_subnet_ids" {
  description = "List of private subnet IDs in the production environment"
  value       = module.vpc.private_subnet_ids
}

output "manager_security_group_id" {
  description = "Security group ID of the manager node"
  value       = module.security_groups.manager_security_group_id
}

output "worker_security_group_id" {
  description = "Security group ID of the worker node"
  value       = module.security_groups.worker_security_group_id
}

output "infra_security_group_id" {
  description = "Security group ID of the infra node"
  value       = module.security_groups.infra_security_group_id
}

output "app_instance_profile_name" {
  description = "IAM instance profile name attached to application EC2 instances"
  value       = module.iam.app_instance_profile_name
}

output "infra_instance_id" {
  description = "EC2 instance ID of the infra node"
  value       = module.infra.instance_id
}

output "infra_private_ip" {
  description = "Private IP address of the infra node"
  value       = module.infra.private_ip
}

output "infra_private_dns" {
  description = "Private DNS name of the infra node"
  value       = module.infra.private_dns
}

output "infra_public_ip" {
  description = "Public IP address of the infra node"
  value       = module.infra.public_ip
}

output "manager_instance_id" {
  description = "EC2 instance ID of the manager node"
  value       = module.manager.instance_id
}

output "manager_public_ip" {
  description = "Public IP address of the manager node (from EIP)"
  value       = module.manager_eip.public_ip
}

output "manager_public_dns" {
  description = "Public DNS name of the manager node (from EIP)"
  value       = module.manager_eip.public_dns
}

output "manager_private_ip" {
  description = "Private IP address of the manager node"
  value       = module.manager.private_ip
}

output "worker_autoscaling_group_name" {
  description = "Name of the worker Auto Scaling group"
  value       = module.worker_asg.autoscaling_group_name
}

output "worker_autoscaling_group_arn" {
  description = "ARN of the worker Auto Scaling group"
  value       = module.worker_asg.autoscaling_group_arn
}

output "worker_desired_capacity" {
  description = "Desired number of worker instances"
  value       = module.worker_asg.desired_capacity
}

output "worker_min_size" {
  description = "Minimum number of worker instances"
  value       = module.worker_asg.min_size
}

output "worker_max_size" {
  description = "Maximum number of worker instances"
  value       = module.worker_asg.max_size
}
