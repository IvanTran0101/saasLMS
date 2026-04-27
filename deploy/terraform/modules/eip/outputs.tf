output "allocation_id" {
  description = "ID that AWS assigns to represent the allocation of the Elastic IP address for use with VPC"
  value       = aws_eip.this.id
}

output "public_ip" {
  description = "Public IP address of the Elastic IP"
  value       = aws_eip.this.public_ip
}

output "public_dns" {
  description = "Public DNS name of the Elastic IP"
  value       = aws_eip.this.public_dns
}

output "private_ip" {
  description = "Private IP address associated with the Elastic IP"
  value       = aws_eip.this.private_ip
}

output "network_interface_id" {
  description = "ID of the network interface associated with the Elastic IP"
  value       = aws_eip.this.network_interface
}

output "instance_id" {
  description = "ID of the instance associated with the Elastic IP"
  value       = aws_eip.this.instance
}