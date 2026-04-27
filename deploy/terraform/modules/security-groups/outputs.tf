output "manager_security_group_id" {
  description = "ID of the manager security group"
  value       = aws_security_group.manager.id
}

output "worker_security_group_id" {
  description = "ID of the worker security group"
  value       = aws_security_group.worker.id
}

output "infra_security_group_id" {
  description = "ID of the infra security group"
  value       = aws_security_group.infra.id
}

output "manager_security_group_name" {
  description = "Name of the manager security group"
  value       = aws_security_group.manager.name
}

output "worker_security_group_name" {
  description = "Name of the worker security group"
  value       = aws_security_group.worker.name
}

output "infra_security_group_name" {
  description = "Name of the infra security group"
  value       = aws_security_group.infra.name
}
