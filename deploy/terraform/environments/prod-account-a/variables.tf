############################
# Project metadata
############################

variable "project_name" {
  description = "Project name used for naming AWS resources."
  type        = string
}

variable "environment" {
  description = "Deployment environment name, for example prod."
  type        = string
}

variable "aws_region" {
  description = "AWS region where resources will be created."
  type        = string
}

variable "aws_profile" {
  description = "AWS shared config/credentials profile used for this environment."
  type        = string
}

############################
# Networking
############################

variable "vpc_cidr" {
  description = "CIDR block for the VPC."
  type        = string
}

variable "public_subnet_cidrs" {
  description = "CIDR blocks for the public subnets."
  type        = list(string)
}

variable "private_subnet_cidrs" {
  description = "CIDR blocks for the private subnets."
  type        = list(string)
}

variable "availability_zones" {
  description = "Availability zones used for the subnets, for example [\"ap-southeast-1a\"]."
  type        = list(string)
}

############################
# EC2 instances
############################

variable "ami_id" {
  description = "AMI ID used for all EC2 instances."
  type        = string
}

variable "key_pair_name" {
  description = "Existing AWS EC2 key pair name used for SSH access."
  type        = string
}

variable "infra_instance_type" {
  description = "EC2 instance type for the infrastructure node."
  type        = string
}

variable "manager_instance_type" {
  description = "EC2 instance type for the Docker Swarm manager node."
  type        = string
}

variable "worker_instance_type" {
  description = "EC2 instance type for the Docker Swarm worker nodes."
  type        = string
}

variable "worker_count" {
  description = "Number of Docker Swarm worker nodes."
  type        = number
}

############################
# Access / SSH
############################

variable "my_ip_cidr" {
  description = "Operator public IP in CIDR format, for example 1.2.3.4/32, used for SSH access."
  type        = string
}

############################
# Domains
############################

variable "domain_name" {
  description = "Root domain name used by the system, for example trananhminh.uk."
  type        = string
}

variable "app_domain" {
  description = "Public domain for the Blazor application."
  type        = string
}

variable "api_domain" {
  description = "Public domain for the Web Gateway."
  type        = string
}

variable "auth_domain" {
  description = "Public domain for the AuthServer."
  type        = string
}

############################
# Storage / IAM options
############################

variable "allow_s3_delete" {
  description = "Whether EC2 application instances are allowed to delete objects from the configured S3 buckets."
  type        = bool
  default     = false
}

############################
# EC2 storage
############################

variable "infra_root_volume_size" {
  description = "Root EBS volume size in GiB for the infrastructure EC2 instance."
  type        = number
}

variable "app_root_volume_size" {
  description = "Root EBS volume size in GiB for the manager and worker EC2 instances."
  type        = number
}

variable "root_volume_type" {
  description = "EBS volume type used for all EC2 root volumes, for example gp3."
  type        = string
}

############################
# S3 / IAM inputs
############################

variable "s3_bucket_names" {
  description = "List of existing S3 bucket names that application EC2 instances need to access."
  type        = list(string)
}

############################
# Common tags
############################

variable "common_tags" {
  description = "Common AWS tags applied to all supported resources."
  type        = map(string)
}
