terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.0"
    }
  }

  required_version = "~> 1.0"

  backend "s3" {
    bucket  = "terraform-state-housing-staging"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/bonuscalc-api/state"
  }
}

provider "aws" {
  region  = "eu-west-2"
}

data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

locals {
  environment     = "staging"
  application     = "bonuscalc-api"
  aws_account     = data.aws_caller_identity.current.account_id
  aws_region      = data.aws_region.current.name
  parameter_store = "arn:aws:ssm:${local.aws_region}:${local.aws_account}:parameter"

  db_name = "bonuscalc"
  db_port = "5432"
}

data "aws_vpc" "default" {
  tags = {
    Name = "housing-stg"
  }
}

data "aws_subnet_ids" "private_subnets" {
  vpc_id = data.aws_vpc.default.id

  tags = {
    Type = "private"
  }
}

data "aws_ssm_parameter" "postgres_password" {
  name = "/${local.application}/${local.environment}/postgres-password"
}

data "aws_ssm_parameter" "postgres_username" {
  name = "/${local.application}/${local.environment}/postgres-username"
}

module "database" {
  source = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/postgres"
  environment_name = local.environment
  vpc_id = data.aws_vpc.default.id
  db_identifier = local.application
  db_name = local.db_name
  db_port = local.db_port
  subnet_ids = data.aws_subnet_ids.private_subnets.ids
  db_engine = "postgres"
  db_engine_version = "16.3"
  db_parameter_group_name = "postgres16"
  db_allow_major_version_upgrade = true
  db_instance_class = "db.t3.micro"
  db_allocated_storage = 20
  maintenance_window = "sun:04:00-sun:04:30"
  db_username = data.aws_ssm_parameter.postgres_username.value
  db_password = data.aws_ssm_parameter.postgres_password.value
  storage_encrypted = true
  multi_az = local.environment == "production"
  publicly_accessible = false
  project_name = "bonus calc"
  additional_tags = {
    BackupPolicy = "Prod"
  }
}
