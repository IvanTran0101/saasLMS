variable "project_name" {
  description = "Project name used for naming security groups"
  type        = string
}

variable "environment" {
  description = "Environment name such as dev, staging, prod"
  type        = string
}

variable "vpc_id" {
  description = "VPC ID where security groups will be created"
  type        = string
}

variable "my_ip_cidr" {
  description = "Your public IP in CIDR format for SSH access, for example 1.2.3.4/32"
  type        = string
}

variable "common_tags" {
  description = "Common tags applied to all security groups"
  type        = map(string)
  default     = {}
}

variable "manager_metrics_ports" {
  description = "Published manager ports that the infra host may scrape for Prometheus metrics"
  type        = list(number)
  default     = [8080, 8082, 8083]
}

variable "monitoring_exporter_ports" {
  description = "Published node/container exporter ports that the infra host may scrape from all Swarm nodes"
  type        = list(number)
  default     = [9100, 8088]
}
