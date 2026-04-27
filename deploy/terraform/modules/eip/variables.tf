variable "project_name" {
  description = "Project name used for naming EIP resources"
  type        = string
}

variable "environment" {
  description = "Environment name such as dev, staging, or prod"
  type        = string
}

variable "name" {
  description = "Logical name of this EIP, for example nat-gateway or bastion"
  type        = string
}

variable "instance_id" {
  description = "ID of the EC2 instance to associate the EIP with (optional)"
  type        = string
  default     = null
}

variable "network_interface_id" {
  description = "ID of the network interface to associate the EIP with (optional)"
  type        = string
  default     = null
}

variable "associate_with_private_ip" {
  description = "Private IP address to associate with the EIP (optional)"
  type        = string
  default     = null
}

variable "common_tags" {
  description = "Common tags to apply to all resources"
  type        = map(string)
  default     = {}
}