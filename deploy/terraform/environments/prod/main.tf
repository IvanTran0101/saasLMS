

module "vpc" {
  source = "../../modules/vpc"

  project_name         = var.project_name
  environment          = var.environment
  vpc_cidr             = var.vpc_cidr
  availability_zones   = var.availability_zones
  public_subnet_cidrs  = var.public_subnet_cidrs
  private_subnet_cidrs = var.private_subnet_cidrs
  enable_dns_support   = true
  enable_dns_hostnames = true
  common_tags          = var.common_tags
}

module "security_groups" {
  source = "../../modules/security-groups"

  project_name = var.project_name
  environment  = var.environment
  vpc_id       = module.vpc.vpc_id
  my_ip_cidr   = var.my_ip_cidr
  common_tags  = var.common_tags
}

module "iam" {
  source = "../../modules/iam"

  project_name    = var.project_name
  environment     = var.environment
  s3_bucket_names = var.s3_bucket_names
  allow_s3_delete = var.allow_s3_delete
  common_tags     = var.common_tags
}

module "infra" {
  source = "../../modules/ec2"

  project_name                = var.project_name
  environment                 = var.environment
  name                        = "infra"
  ami_id                      = var.ami_id
  instance_type               = var.infra_instance_type
  subnet_id                   = module.vpc.public_subnet_ids[0]
  security_group_ids          = [module.security_groups.infra_security_group_id]
  key_pair_name               = var.key_pair_name
  associate_public_ip_address = true
  iam_instance_profile_name   = null
  root_volume_size            = var.infra_root_volume_size
  root_volume_type            = var.root_volume_type
  common_tags                 = var.common_tags
}

module "manager" {
  source = "../../modules/ec2"

  project_name       = var.project_name
  environment        = var.environment
  name               = "manager"
  ami_id             = var.ami_id
  instance_type      = var.manager_instance_type
  subnet_id          = module.vpc.public_subnet_ids[0]
  security_group_ids = [module.security_groups.manager_security_group_id]
  key_pair_name      = var.key_pair_name
  # Manager is reachable via an Elastic IP; keep a public IPv4 on the instance as well
  # to avoid drift/replacement if a public IP gets associated out-of-band.
  associate_public_ip_address = true
  iam_instance_profile_name   = module.iam.app_instance_profile_name
  root_volume_size            = var.app_root_volume_size
  root_volume_type            = var.root_volume_type
  common_tags                 = var.common_tags
}

module "manager_eip" {
  source = "../../modules/eip"

  project_name = var.project_name
  environment  = var.environment
  name         = "manager"
  instance_id  = module.manager.instance_id
  common_tags  = var.common_tags
}

module "worker_asg" {
  source = "../../modules/asg-workers"

  project_name                               = var.project_name
  environment                                = var.environment
  name                                       = "worker"
  ami_id                                     = var.ami_id
  instance_type                              = var.worker_instance_type
  subnet_ids                                 = module.vpc.public_subnet_ids
  security_group_ids                         = [module.security_groups.worker_security_group_id]
  key_pair_name                              = var.key_pair_name
  iam_instance_profile_name                  = module.iam.app_instance_profile_name
  associate_public_ip_address                = true
  desired_capacity                           = var.worker_count
  min_size                                   = var.worker_count
  max_size                                   = max(var.worker_count, 2)
  root_volume_size                           = var.app_root_volume_size
  root_volume_type                           = var.root_volume_type
  common_tags                                = var.common_tags
  aws_region                                 = var.aws_region
  swarm_manager_addr_ssm_parameter_name      = "/${var.project_name}/${var.environment}/swarm/manager-join-addr"
  swarm_worker_join_token_ssm_parameter_name = "/${var.project_name}/${var.environment}/swarm/worker-join-token"
}
