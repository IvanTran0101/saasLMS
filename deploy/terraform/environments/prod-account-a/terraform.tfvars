project_name = "saaslms"
environment  = "prod-account-a"
aws_region   = "ap-southeast-1"
aws_profile  = "account-a"

vpc_cidr             = "10.0.0.0/16"
public_subnet_cidrs  = ["10.0.1.0/24"]
private_subnet_cidrs = ["10.0.2.0/24"]
availability_zones   = ["ap-southeast-1a"]

ami_id                = "ami-0c1d28734eb221b6d"
key_pair_name         = "saaslms"
infra_instance_type   = "t3.medium"
manager_instance_type = "t3.medium"
worker_instance_type  = "t3.small"
worker_count          = 1

my_ip_cidr = "42.115.43.69/32"

domain_name = "trananhminh.uk"
app_domain  = "app.trananhminh.uk"
api_domain  = "api.trananhminh.uk"
auth_domain = "auth.trananhminh.uk"

s3_bucket_names = [
  "lms-assess-sub-dev-864946423771-ap-southeast-1-an",
  "lms-course-mat-dev-864946423771-ap-southeast-1-an"
]

common_tags = {
  Project     = "saaslms"
  Environment = "prod-account-a"
  Owner       = "trananhminh"
}
allow_s3_delete        = false
infra_root_volume_size = 50
app_root_volume_size   = 30
root_volume_type       = "gp3"
