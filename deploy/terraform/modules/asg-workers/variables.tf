variable "project_name" {
  description = "Project name used for naming ASG resources"
  type        = string
}

variable "environment" {
  description = "Environment name such as dev, staging, or prod"
  type        = string
}

variable "name" {
  description = "Logical name for the worker group"
  type        = string
  default     = "worker"
}

variable "ami_id" {
  description = "AMI ID used by worker instances"
  type        = string
}

variable "instance_type" {
  description = "EC2 instance type for worker instances"
  type        = string
}

variable "subnet_ids" {
  description = "Subnets used by the Auto Scaling group"
  type        = list(string)
}

variable "security_group_ids" {
  description = "Security groups attached to worker instances"
  type        = list(string)
}

variable "key_pair_name" {
  description = "Existing EC2 key pair name used for SSH access"
  type        = string
}

variable "iam_instance_profile_name" {
  description = "IAM instance profile attached to worker instances"
  type        = string
}

variable "associate_public_ip_address" {
  description = "Whether workers should have public IPv4 addresses"
  type        = bool
  default     = true
}

variable "desired_capacity" {
  description = "Desired number of worker instances"
  type        = number
}

variable "min_size" {
  description = "Minimum number of worker instances"
  type        = number
}

variable "max_size" {
  description = "Maximum number of worker instances"
  type        = number
}

variable "root_volume_size" {
  description = "Size of worker root EBS volume in GiB"
  type        = number
  default     = 30
}

variable "root_volume_type" {
  description = "Type of worker root EBS volume"
  type        = string
  default     = "gp3"
}

variable "common_tags" {
  description = "Common tags applied to the worker resources"
  type        = map(string)
  default     = {}
}

variable "aws_region" {
  description = "AWS region used by the worker bootstrap script"
  type        = string
}

variable "swarm_manager_addr_ssm_parameter_name" {
  description = "SSM parameter name that stores the Docker Swarm manager join address"
  type        = string
}

variable "swarm_worker_join_token_ssm_parameter_name" {
  description = "SSM parameter name that stores the Docker Swarm worker join token"
  type        = string
}
