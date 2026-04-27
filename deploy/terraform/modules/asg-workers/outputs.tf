output "launch_template_id" {
  description = "ID of the launch template used by worker ASG"
  value       = aws_launch_template.this.id
}

output "launch_template_latest_version" {
  description = "Latest launch template version used by worker ASG"
  value       = aws_launch_template.this.latest_version
}

output "autoscaling_group_name" {
  description = "Name of the worker Auto Scaling group"
  value       = aws_autoscaling_group.this.name
}

output "autoscaling_group_arn" {
  description = "ARN of the worker Auto Scaling group"
  value       = aws_autoscaling_group.this.arn
}

output "desired_capacity" {
  description = "Desired number of instances in worker Auto Scaling group"
  value       = aws_autoscaling_group.this.desired_capacity
}

output "min_size" {
  description = "Minimum number of instances in worker Auto Scaling group"
  value       = aws_autoscaling_group.this.min_size
}

output "max_size" {
  description = "Maximum number of instances in worker Auto Scaling group"
  value       = aws_autoscaling_group.this.max_size
}
