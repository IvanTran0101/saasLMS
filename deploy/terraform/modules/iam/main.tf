data "aws_iam_policy_document" "ec2_assume_role" {
  statement {
    effect = "Allow"

    principals {
      type        = "Service"
      identifiers = ["ec2.amazonaws.com"]
    }

    actions = ["sts:AssumeRole"]
  }
}

resource "aws_iam_role" "app_ec2_role" {
  name               = "${var.project_name}-${var.environment}-app-ec2-role"
  assume_role_policy = data.aws_iam_policy_document.ec2_assume_role.json

  tags = merge(var.common_tags, {
    Name = "${var.project_name}-${var.environment}-app-ec2-role"
    Role = "app-ec2"
  })
}

resource "aws_iam_policy" "s3_bucket_access" {
  name        = "${var.project_name}-${var.environment}-s3-bucket-access"
  description = "Allow EC2 application instances to access the configured S3 buckets"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = concat(
      [
        {
          Effect = "Allow"
          Action = [
            "s3:ListBucket"
          ]
          Resource = [for bucket_name in var.s3_bucket_names : "arn:aws:s3:::${bucket_name}"]
        },
        {
          Effect = "Allow"
          Action = [
            "s3:GetObject",
            "s3:PutObject"
          ]
          Resource = [for bucket_name in var.s3_bucket_names : "arn:aws:s3:::${bucket_name}/*"]
        }
      ],
      var.allow_s3_delete ? [
        {
          Effect = "Allow"
          Action = [
            "s3:DeleteObject"
          ]
          Resource = [for bucket_name in var.s3_bucket_names : "arn:aws:s3:::${bucket_name}/*"]
        }
      ] : []
    )
  })

  tags = merge(var.common_tags, {
    Name = "${var.project_name}-${var.environment}-s3-bucket-access"
  })
}

resource "aws_iam_role_policy_attachment" "s3_bucket_access" {
  role       = aws_iam_role.app_ec2_role.name
  policy_arn = aws_iam_policy.s3_bucket_access.arn
}

resource "aws_iam_policy" "ec2_metadata_options" {
  name        = "${var.project_name}-${var.environment}-ec2-metadata-options"
  description = "Allow app EC2 instances to adjust their own instance metadata options (IMDS) when needed"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "ec2:ModifyInstanceMetadataOptions",
          "ec2:DescribeInstances"
        ]
        Resource = "arn:aws:ec2:*:*:instance/*"
        Condition = {
          StringEquals = {
            "aws:ResourceTag/Project"     = var.project_name,
            "aws:ResourceTag/Environment" = var.environment
          }
        }
      }
    ]
  })

  tags = merge(var.common_tags, {
    Name = "${var.project_name}-${var.environment}-ec2-metadata-options"
  })
}

resource "aws_iam_role_policy_attachment" "ec2_metadata_options" {
  role       = aws_iam_role.app_ec2_role.name
  policy_arn = aws_iam_policy.ec2_metadata_options.arn
}

resource "aws_iam_policy" "swarm_bootstrap_parameter_read" {
  name        = "${var.project_name}-${var.environment}-swarm-bootstrap-parameter-read"
  description = "Allow EC2 application instances to read and publish Swarm bootstrap parameters in SSM"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "ssm:PutParameter",
          "ssm:GetParameter",
          "ssm:GetParameters"
        ]
        Resource = [
          "arn:aws:ssm:*:*:parameter/${var.project_name}/${var.environment}/swarm/*"
        ]
      }
    ]
  })

  tags = merge(var.common_tags, {
    Name = "${var.project_name}-${var.environment}-swarm-bootstrap-parameter-read"
  })
}

resource "aws_iam_role_policy_attachment" "swarm_bootstrap_parameter_read" {
  role       = aws_iam_role.app_ec2_role.name
  policy_arn = aws_iam_policy.swarm_bootstrap_parameter_read.arn
}

resource "aws_iam_instance_profile" "app_instance_profile" {
  name = "${var.project_name}-${var.environment}-app-instance-profile"
  role = aws_iam_role.app_ec2_role.name

  tags = merge(var.common_tags, {
    Name = "${var.project_name}-${var.environment}-app-instance-profile"
  })
}
