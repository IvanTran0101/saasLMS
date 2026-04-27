resource "aws_eip" "this" {
  instance                  = var.instance_id
  network_interface         = var.network_interface_id
  associate_with_private_ip = var.associate_with_private_ip

  tags = merge(var.common_tags, {
    Name = "${var.project_name}-${var.environment}-${var.name}"
  })
}