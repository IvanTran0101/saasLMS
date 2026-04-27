resource "aws_launch_template" "this" {
  name_prefix   = "${var.project_name}-${var.environment}-${var.name}-"
  image_id      = var.ami_id
  instance_type = var.instance_type
  key_name      = var.key_pair_name
  user_data = base64encode(templatefile("${path.module}/templates/worker-user-data.sh.tftpl", {
    aws_region                                 = var.aws_region
    swarm_manager_addr_ssm_parameter_name      = var.swarm_manager_addr_ssm_parameter_name
    swarm_worker_join_token_ssm_parameter_name = var.swarm_worker_join_token_ssm_parameter_name
  }))

  iam_instance_profile {
    name = var.iam_instance_profile_name
  }

  metadata_options {
    http_endpoint               = "enabled"
    http_tokens                 = "required"
    http_put_response_hop_limit = 2
  }

  network_interfaces {
    associate_public_ip_address = var.associate_public_ip_address
    security_groups             = var.security_group_ids
  }

  block_device_mappings {
    device_name = "/dev/sda1"

    ebs {
      volume_size           = var.root_volume_size
      volume_type           = var.root_volume_type
      encrypted             = true
      delete_on_termination = true
    }
  }

  tag_specifications {
    resource_type = "instance"
    tags = merge(var.common_tags, {
      Name = "${var.project_name}-${var.environment}-${var.name}"
      Role = var.name
    })
  }

  tag_specifications {
    resource_type = "volume"
    tags = merge(var.common_tags, {
      Name = "${var.project_name}-${var.environment}-${var.name}"
      Role = var.name
    })
  }

  tags = merge(var.common_tags, {
    Name = "${var.project_name}-${var.environment}-${var.name}-lt"
    Role = var.name
  })
}

resource "aws_autoscaling_group" "this" {
  name                = "${var.project_name}-${var.environment}-${var.name}-asg"
  desired_capacity    = var.desired_capacity
  min_size            = var.min_size
  max_size            = var.max_size
  vpc_zone_identifier = var.subnet_ids
  health_check_type   = "EC2"

  launch_template {
    id      = aws_launch_template.this.id
    version = "$Latest"
  }

  tag {
    key                 = "Name"
    value               = "${var.project_name}-${var.environment}-${var.name}"
    propagate_at_launch = true
  }

  tag {
    key                 = "Role"
    value               = var.name
    propagate_at_launch = true
  }

  dynamic "tag" {
    for_each = var.common_tags
    content {
      key                 = tag.key
      value               = tag.value
      propagate_at_launch = true
    }
  }
}

resource "aws_autoscaling_policy" "cpu_target_tracking" {
  name                   = "${var.project_name}-${var.environment}-${var.name}-cpu-target"
  policy_type            = "TargetTrackingScaling"
  autoscaling_group_name = aws_autoscaling_group.this.name

  estimated_instance_warmup = 240

  target_tracking_configuration {
    predefined_metric_specification {
      predefined_metric_type = "ASGAverageCPUUtilization"
    }

    target_value = 55
  }
}
