

resource "aws_security_group" "manager" {
  name        = "${var.project_name}-${var.environment}-manager-sg"
  description = "Security group for Docker Swarm manager node"
  vpc_id      = var.vpc_id

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = merge(var.common_tags, {
    Name = "${var.project_name}-${var.environment}-manager-sg"
    Role = "manager"
  })
}

resource "aws_security_group" "worker" {
  name        = "${var.project_name}-${var.environment}-worker-sg"
  description = "Security group for Docker Swarm worker nodes"
  vpc_id      = var.vpc_id

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = merge(var.common_tags, {
    Name = "${var.project_name}-${var.environment}-worker-sg"
    Role = "worker"
  })
}

resource "aws_security_group" "infra" {
  name        = "${var.project_name}-${var.environment}-infra-sg"
  description = "Security group for infra node hosting SQL Server, Redis, and RabbitMQ"
  vpc_id      = var.vpc_id

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = merge(var.common_tags, {
    Name = "${var.project_name}-${var.environment}-infra-sg"
    Role = "infra"
  })
}

resource "aws_security_group_rule" "manager_http_ingress" {
  type              = "ingress"
  description       = "Allow HTTP traffic from the internet"
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.manager.id
}

resource "aws_security_group_rule" "manager_https_ingress" {
  type              = "ingress"
  description       = "Allow HTTPS traffic from the internet"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.manager.id
}

resource "aws_security_group_rule" "manager_metrics_from_infra" {
  for_each = toset([for port in var.manager_metrics_ports : tostring(port)])

  type                     = "ingress"
  description              = "Allow Prometheus scrapes from infra node to manager published metrics ports"
  from_port                = tonumber(each.value)
  to_port                  = tonumber(each.value)
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.infra.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "manager_monitoring_exporters_from_infra" {
  for_each = toset([for port in var.monitoring_exporter_ports : tostring(port)])

  type                     = "ingress"
  description              = "Allow Prometheus scrapes from infra node to manager exporters"
  from_port                = tonumber(each.value)
  to_port                  = tonumber(each.value)
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.infra.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "manager_ssh_ingress" {
  type              = "ingress"
  description       = "Allow SSH access to manager from operator IP"
  from_port         = 22
  to_port           = 22
  protocol          = "tcp"
  cidr_blocks       = [var.my_ip_cidr]
  security_group_id = aws_security_group.manager.id
}

resource "aws_security_group_rule" "worker_ssh_ingress" {
  type              = "ingress"
  description       = "Allow SSH access to worker from operator IP"
  from_port         = 22
  to_port           = 22
  protocol          = "tcp"
  cidr_blocks       = [var.my_ip_cidr]
  security_group_id = aws_security_group.worker.id
}

resource "aws_security_group_rule" "infra_ssh_ingress" {
  type              = "ingress"
  description       = "Allow SSH access to infra from operator IP"
  from_port         = 22
  to_port           = 22
  protocol          = "tcp"
  cidr_blocks       = [var.my_ip_cidr]
  security_group_id = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_ssh_from_manager" {
  type                     = "ingress"
  description              = "Allow SSH access to infra from manager nodes"
  from_port                = 22
  to_port                  = 22
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "worker_ssh_from_manager" {
  type                     = "ingress"
  description              = "Allow SSH access to worker from manager nodes"
  from_port                = 22
  to_port                  = 22
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.worker.id
}

resource "aws_security_group_rule" "worker_monitoring_exporters_from_infra" {
  for_each = toset([for port in var.monitoring_exporter_ports : tostring(port)])

  type                     = "ingress"
  description              = "Allow Prometheus scrapes from infra node to worker exporters"
  from_port                = tonumber(each.value)
  to_port                  = tonumber(each.value)
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.infra.id
  security_group_id        = aws_security_group.worker.id
}

resource "aws_security_group_rule" "manager_all_from_manager" {
  type                     = "ingress"
  description              = "Allow all internal traffic from manager nodes to manager nodes"
  from_port                = 0
  to_port                  = 0
  protocol                 = "-1"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "manager_all_from_worker" {
  type                     = "ingress"
  description              = "Allow all internal traffic from worker nodes to manager nodes"
  from_port                = 0
  to_port                  = 0
  protocol                 = "-1"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "worker_all_from_manager" {
  type                     = "ingress"
  description              = "Allow all internal traffic from manager nodes to worker nodes"
  from_port                = 0
  to_port                  = 0
  protocol                 = "-1"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.worker.id
}

resource "aws_security_group_rule" "worker_all_from_worker" {
  type                     = "ingress"
  description              = "Allow all internal traffic from worker nodes to worker nodes"
  from_port                = 0
  to_port                  = 0
  protocol                 = "-1"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.worker.id
}

resource "aws_security_group_rule" "manager_swarm_management_from_manager" {
  type                     = "ingress"
  description              = "Allow Docker Swarm management traffic from manager group"
  from_port                = 2377
  to_port                  = 2377
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "manager_swarm_management_from_worker" {
  type                     = "ingress"
  description              = "Allow Docker Swarm management traffic from worker group"
  from_port                = 2377
  to_port                  = 2377
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "manager_gossip_tcp_from_manager" {
  type                     = "ingress"
  description              = "Allow Docker Swarm gossip TCP traffic from manager group"
  from_port                = 7946
  to_port                  = 7946
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "manager_gossip_tcp_from_worker" {
  type                     = "ingress"
  description              = "Allow Docker Swarm gossip TCP traffic from worker group"
  from_port                = 7946
  to_port                  = 7946
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "worker_gossip_tcp_from_manager" {
  type                     = "ingress"
  description              = "Allow Docker Swarm gossip TCP traffic from manager group"
  from_port                = 7946
  to_port                  = 7946
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.worker.id
}

resource "aws_security_group_rule" "worker_gossip_tcp_from_worker" {
  type                     = "ingress"
  description              = "Allow Docker Swarm gossip TCP traffic from worker group"
  from_port                = 7946
  to_port                  = 7946
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.worker.id
}

resource "aws_security_group_rule" "manager_gossip_udp_from_manager" {
  type                     = "ingress"
  description              = "Allow Docker Swarm gossip UDP traffic from manager group"
  from_port                = 7946
  to_port                  = 7946
  protocol                 = "udp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "manager_gossip_udp_from_worker" {
  type                     = "ingress"
  description              = "Allow Docker Swarm gossip UDP traffic from worker group"
  from_port                = 7946
  to_port                  = 7946
  protocol                 = "udp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "worker_gossip_udp_from_manager" {
  type                     = "ingress"
  description              = "Allow Docker Swarm gossip UDP traffic from manager group"
  from_port                = 7946
  to_port                  = 7946
  protocol                 = "udp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.worker.id
}

resource "aws_security_group_rule" "worker_gossip_udp_from_worker" {
  type                     = "ingress"
  description              = "Allow Docker Swarm gossip UDP traffic from worker group"
  from_port                = 7946
  to_port                  = 7946
  protocol                 = "udp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.worker.id
}

resource "aws_security_group_rule" "manager_overlay_udp_from_manager" {
  type                     = "ingress"
  description              = "Allow Docker overlay network UDP traffic from manager group"
  from_port                = 4789
  to_port                  = 4789
  protocol                 = "udp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "manager_overlay_udp_from_worker" {
  type                     = "ingress"
  description              = "Allow Docker overlay network UDP traffic from worker group"
  from_port                = 4789
  to_port                  = 4789
  protocol                 = "udp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.manager.id
}

resource "aws_security_group_rule" "worker_overlay_udp_from_manager" {
  type                     = "ingress"
  description              = "Allow Docker overlay network UDP traffic from manager group"
  from_port                = 4789
  to_port                  = 4789
  protocol                 = "udp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.worker.id
}

resource "aws_security_group_rule" "worker_overlay_udp_from_worker" {
  type                     = "ingress"
  description              = "Allow Docker overlay network UDP traffic from worker group"
  from_port                = 4789
  to_port                  = 4789
  protocol                 = "udp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.worker.id
}

resource "aws_security_group_rule" "infra_sqlserver_from_manager" {
  type                     = "ingress"
  description              = "Allow SQL Server access from manager nodes"
  from_port                = 1434
  to_port                  = 1434
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_sqlserver_from_worker" {
  type                     = "ingress"
  description              = "Allow SQL Server access from worker nodes"
  from_port                = 1434
  to_port                  = 1434
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_redis_from_manager" {
  type                     = "ingress"
  description              = "Allow Redis access from manager nodes"
  from_port                = 6379
  to_port                  = 6379
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_redis_from_worker" {
  type                     = "ingress"
  description              = "Allow Redis access from worker nodes"
  from_port                = 6379
  to_port                  = 6379
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_rabbitmq_from_manager" {
  type                     = "ingress"
  description              = "Allow RabbitMQ access from manager nodes"
  from_port                = 5672
  to_port                  = 5672
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_rabbitmq_from_worker" {
  type                     = "ingress"
  description              = "Allow RabbitMQ access from worker nodes"
  from_port                = 5672
  to_port                  = 5672
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_elasticsearch_from_manager" {
  type                     = "ingress"
  description              = "Allow Elasticsearch access from manager nodes"
  from_port                = 9200
  to_port                  = 9200
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_elasticsearch_from_worker" {
  type                     = "ingress"
  description              = "Allow Elasticsearch access from worker nodes"
  from_port                = 9200
  to_port                  = 9200
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_rabbitmq_ui_from_operator" {
  type              = "ingress"
  description       = "Allow RabbitMQ management UI access from operator IP"
  from_port         = 15672
  to_port           = 15672
  protocol          = "tcp"
  cidr_blocks       = [var.my_ip_cidr]
  security_group_id = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_kibana_from_operator" {
  type              = "ingress"
  description       = "Allow Kibana access from operator IP"
  from_port         = 5601
  to_port           = 5601
  protocol          = "tcp"
  cidr_blocks       = [var.my_ip_cidr]
  security_group_id = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_grafana_from_operator" {
  type              = "ingress"
  description       = "Allow Grafana access from operator IP"
  from_port         = 3000
  to_port           = 3000
  protocol          = "tcp"
  cidr_blocks       = [var.my_ip_cidr]
  security_group_id = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_grafana_from_manager" {
  type                     = "ingress"
  description              = "Allow Grafana access from manager nodes"
  from_port                = 3000
  to_port                  = 3000
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_prometheus_from_operator" {
  type              = "ingress"
  description       = "Allow Prometheus access from operator IP"
  from_port         = 9090
  to_port           = 9090
  protocol          = "tcp"
  cidr_blocks       = [var.my_ip_cidr]
  security_group_id = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_prometheus_from_manager" {
  type                     = "ingress"
  description              = "Allow Prometheus access from manager nodes"
  from_port                = 9090
  to_port                  = 9090
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_loki_from_manager" {
  type                     = "ingress"
  description              = "Allow Loki log ingestion from manager nodes"
  from_port                = 3100
  to_port                  = 3100
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.manager.id
  security_group_id        = aws_security_group.infra.id
}

resource "aws_security_group_rule" "infra_loki_from_worker" {
  type                     = "ingress"
  description              = "Allow Loki log ingestion from worker nodes"
  from_port                = 3100
  to_port                  = 3100
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.worker.id
  security_group_id        = aws_security_group.infra.id
}
