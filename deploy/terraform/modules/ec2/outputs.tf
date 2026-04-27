output "instance_id" {
  description = "ID of the EC2 instance"
  value       = aws_instance.this.id
}

output "instance_arn" {
  description = "ARN of the EC2 instance"
  value       = aws_instance.this.arn
}

output "private_ip" {
  description = "Private IP address of the EC2 instance"
  value       = aws_instance.this.private_ip
}

output "private_dns" {
  description = "Private DNS name of the EC2 instance"
  value       = aws_instance.this.private_dns
}

output "public_ip" {
  description = "Public IP address of the EC2 instance, if assigned"
  value       = aws_instance.this.public_ip
}

output "public_dns" {
  description = "Public DNS name of the EC2 instance, if assigned"
  value       = aws_instance.this.public_dns
}

output "availability_zone" {
  description = "Availability zone where the EC2 instance is deployed"
  value       = aws_instance.this.availability_zone
}
