

variable "project_name" {
  description = "Project name used for naming EC2 resources"
  type        = string
}

variable "environment" {
  description = "Environment name such as dev, staging, or prod"
  type        = string
}

variable "name" {
  description = "Logical name of this EC2 instance, for example infra, manager, or worker-1"
  type        = string
}

variable "ami_id" {
  description = "AMI ID used for the EC2 instance"
  type        = string
}

variable "instance_type" {
  description = "EC2 instance type"
  type        = string
}

variable "subnet_id" {
  description = "Subnet ID where the EC2 instance will be launched"
  type        = string
}

variable "security_group_ids" {
  description = "List of security group IDs attached to the EC2 instance"
  type        = list(string)
}

variable "key_pair_name" {
  description = "Existing EC2 key pair name used for SSH access"
  type        = string
}

variable "associate_public_ip_address" {
  description = "Whether to associate a public IP address with the EC2 instance"
  type        = bool
  default     = false
}

variable "iam_instance_profile_name" {
  description = "Optional IAM instance profile name to attach to the EC2 instance"
  type        = string
  default     = null
}

variable "root_volume_size" {
  description = "Size of the root EBS volume in GiB"
  type        = number
  default     = 30
}

variable "root_volume_type" {
  description = "Type of the root EBS volume"
  type        = string
  default     = "gp3"
}

variable "common_tags" {
  description = "Common tags applied to the EC2 instance and related resources"
  type        = map(string)
  default     = {}
}