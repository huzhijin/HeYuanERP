# 纳力OA 备份与应急运维方案

- 文档版本：v1.1
- 编制日期：2025-09-17
- 适用系统：致远OA（Seeyon）+ MySQL + 深信服超融合（HCI）
- 责任部门：信息化中心（系统/数据库/存储/安全）

> 目的：对“灾难性恢复方案与指导书”的要求，明确备份与容灾策略、操作规程、应急处置与演练台账，覆盖应用、数据库、附件与基础设施层。本文已按“每日除快照外，执行异机备份（数据库+附件）”的要求进行设计。

---

## 1. 目标与范围

- 目标
  - 确保致远OA在故障或灾难发生时可在目标时间内恢复，数据丢失在可接受范围。
  - 提供可操作的日常备份、异地/异机副本、演练与应急处置指南，并形成可审计的证据与台账。
- 范围
  - OA 应用（Tomcat/Java）、配置与日志
  - MySQL 数据库实例及其配置、binlog
  - 附件/正文/导入导出/全文检索索引等文件目录
  - 深信服 HCI 一致性组、快照与复制能力，及备份介质（本地/同城/异地）

## 2. 系统架构与资产清单

- 关键组件
  - 致远OA应用：应用服务、配置文件、计划任务/定时器
  - 数据库：MySQL（单机或主从/复制）
  - 文件资产：附件、正文、图片、导入导出、全文检索索引
  - 附件服务器：独立承载附件/正文的服务器或 NAS（NFS/SMB/iSCSI）
  - 基础设施：深信服 HCI（计算/存储/网络）；当前无独立反向代理/Nginx（应用直连）
- 重要路径与参数（上线时补齐）
  - OA 附件目录：`<OA_ATTACH_DIR>`（系统参数-文件存储路径为准）
  - OA 导入导出目录：`<OA_EXPORT_DIR>`
  - OA 索引目录：`<OA_INDEX_DIR>`（可重建）
  - MySQL 数据目录：`<MYSQL_DATA_DIR>`；配置：`/etc/my.cnf`
  - 备份主机：`<BACKUP_SERVER>`；备份根：`<BACKUP_BASE>`（例：`/backup`）

## 3. 恢复目标（RTO/RPO）

- OA 生产故障（单站）：RTO ≤ 4小时，RPO ≤ 15分钟
- 区域性灾难（同城/异地）：RTO ≤ 8小时，RPO ≤ 1小时
- 低优先级组件（全文索引等）：RTO ≤ 24小时，RPO ≤ 24小时

## 4. 策略总览（3-2-1 原则）

- 至少3份副本、2种介质、1份异地/离线。
- 分层实施：
  - 基础设施层：HCI 一致性组快照（含应用VM+DB VM），每4小时；同城/异地复制 15–60 分钟。
  - 数据库层：每日异机备份（xtrabackup 增量或全量）+ 持续 binlog；每周逻辑备份（跨版本恢复）。
  - 文件层（附件/正文等）：每日异机 rsync 增量 + 硬链接快照；与数据库保持同一时间点。
  - 月度/长期：不可变/对象存储留存，满足审计与合规要求。

## 5. 日常备份设计（含“每日异机”）

### 5.1 HCI 层（深信服）
- 一致性组：将 OA 应用VM 与 MySQL VM 纳入同一 CG，启用应用一致性（Guest Tools 或脚本 quiesce）。
- 快照：每 4 小时 1 次，保留 7–14 天；每日 1 次日结快照保留 30 天。
- 复制：同城/异地异步复制（15–60 分钟），DR 侧保留 ≥ 7 天。
- 用途：小范围回滚、快速挂载导出；重大灾难作为温备基础。

### 5.2 数据库层（MySQL）
- 配置基线
  - 开启 binlog（ROW）、GTID；`innodb_flush_log_at_trx_commit=1`、`sync_binlog=1`
  - binlog 保留：覆盖 RPO 目标 + 安全余量（如 ≥ 7 天）
- 备份策略（每日异机）
  - 每周日 01:30 全量物理备份（xtrabackup）至备份主机（异机存储）
  - 周一至周六 01:30 增量物理备份（基于最近全量/增量）至备份主机
  - 每日 03:30 逻辑备份（mysqldump）保留 1 份/天（用于跨版本/校验）
  - 持续 binlog 上传/同步至备份主机（每小时校验完整性）
- 备份示例命令
  - 全量：`xtrabackup --backup --target-dir=$BACKUP_BASE/mysql/full-$(date +%F)`
  - 增量：`xtrabackup --backup --incremental-basedir=$BACKUP_BASE/mysql/last --target-dir=$BACKUP_BASE/mysql/inc-$(date +%F-%H)`
  - 准备：`xtrabackup --prepare --target-dir=...`（按 inc→full 顺序）
  - 逻辑：`mysqldump --single-transaction --routines --triggers --events --hex-blob --master-data=2 -u backup -p seeyon > $BACKUP_BASE/mysql/dump/seeyon_$(date +%F).sql`
- 传输与安全（到异机）
  - 通道：专线/专用VLAN；rsync/SSH；开启压缩与限速
  - 加密：静态 GPG/AES-256；传输 TLS/SSH；备份账号最小权限
- 恢复校验
  - 抽检 `CHECKSUM TABLE`、关键业务表；对比最近公文/流程样本

### 5.3 文件层（附件/正文/导入导出）
- 同步策略（每日异机）
  - 每日 02:30 从生产 `OA_ATTACH_DIR` 增量 rsync 至备份主机 `$BACKUP_BASE/files/current`，并以硬链接制作只读“类快照”目录
  - 与数据库一致性：备份窗口前短暂停用写密集任务，或配合 HCI 一致性快照时间点
- 示例命令
  - `rsync -a --delete --link-dest=$BACKUP_BASE/files/latest <OA_ATTACH_DIR>/ $BACKUP_BASE/files/snap-$(date +%F)/ && ln -sfn $BACKUP_BASE/files/snap-$(date +%F) $BACKUP_BASE/files/latest`
- 索引目录
  - 全文检索索引可在恢复后重建，无需长期备份
  
> 若附件目录部署在“附件服务器”上，请参考第 17 节的专用方案（含 Push/Pull 两种模式、NAS 快照/复制与应急切换流程）。

### 5.4 配置与其他
- 备份 OA 配置、计划任务清单、脚本与版本信息至 `$BACKUP_BASE/conf/`（每日同步）
- 生成每日备份报告（校验和/容量/成功率），邮件/IM 推送

## 6. 备份介质与安全

- 介质
  - 本地：HCI 存储快照
  - 异机：备份主机本地磁盘或 iSCSI/NFS 卷（同城/异地）
  - 长期：对象存储（开启对象锁/WORM）或离线介质（磁带/离线硬盘）
- 安全
  - 备份加密（AES-256/GPG）；密钥托管（金库/双人双控）
  - 访问控制：白名单IP、CHAP（如 iSCSI）、最小权限账户、MFA
  - 不可变：关键月度留存采用对象存储对象锁/WORM；设置防删除窗口
  - 审计：操作留痕、变更审批、双人复核

## 7. 监控与告警

- 监控项：备份任务成功率、容量增长、binlog 滞后、HCI 复制延迟、VM/存储健康、应用心跳（登录页/关键接口）
- 告警：短信/邮件/企业微信/钉钉；分级响应（15/30/60 分钟）

## 8. 应急恢复流程（Runbook）

### 8.1 常见场景
- 误删/逻辑损坏（应用可用）：冻结写入→选定 PITR 时间点→在隔离环境恢复验证→回切至生产或定向回表→验证→恢复服务
- 勒索/入侵：隔离网络→切断备份写入→选择“已校验”的不可变/异地副本恢复→杀毒补丁→密钥/口令轮换→溯源复盘
- 单 VM/主机故障：HCI HA 迁移或快照回滚→应用健康检查→对外恢复
- 存储卷损坏：挂载 HCI 快照/复制副本恢复→完整性校验
- 机房级中断：启动 DR 站点→按“数据库→附件→应用→入口/DNS”顺序恢复→宣布切换→业务联调→制定回切

### 8.2 数据库恢复（示例）
1) 在 DR/备份主机执行 xtrabackup 全量+增量 `--prepare`
2) 恢复数据目录至目标实例，确保权限、`my.cnf` 一致
3) 应用 binlog 至指定时间点：`mysqlbinlog --start-datetime="YYYY-MM-DD HH:MM:SS" binlog.* | mysql -uroot -p`
4) 启动 MySQL，校验业务数据抽样

### 8.3 附件恢复（示例）
1) 挂载对应日期的 `files/snap-YYYY-MM-DD` 目录
2) 回拷至生产或以绑定存储替换（确保权限/属主）
3) 与数据库时间点一致性核对（随机抽检公文/附件可打开）

### 8.4 应用恢复（示例）
1) 部署对应版本 JDK/Tomcat/致远OA，导入配置与 License
2) 修正数据库连接、附件路径、访问入口/DNS（如有）；当前架构为应用直连，无独立 Nginx 配置
3) 验证登录、流程发起与附件预览；无误后开放全部用户

## 9. 演练计划与台账

- 周期：季度 1 次（同城/异地DR测试启动）；年度 1 次全流程演练（含回切）
- 记录：演练日期、场景、RTO/RPO 实测、问题与改进、签字
- 证据：演练报告、截图/日志、恢复校验记录、备份文件校验和

## 10. 运维职责与分工

- 系统负责人：批准 RTO/RPO、演练与重大变更
- DBA：数据库备份/恢复、PITR、校验
- 应用运维：OA 配置/附件备份、应用恢复与联调
- 存储/HCI：一致性组、快照/复制策略与演练
- 安全与审计：密钥/访问控制、日志与审计报告

## 11. 合规与审计要点（安永关注）

- 3-2-1 策略落实与证据（多副本、第二介质、异地/不可变）
- 访问控制与最小权限（含备份账号、密钥管理）
- 不可变存储/对象锁策略（防勒索/误删）
- 演练与恢复可用性证明（RTO/RPO 实测、抽检记录）
- 变更留痕与双人复核（脚本/计划任务/策略改动均需审批）

> 注：当前架构不使用独立 Nginx/反向代理，访问入口为应用直连；若未来引入负载均衡或网关，需同步纳入备份/恢复与变更台账。

## 12. 示例计划（可直接落地）

```cron
# MySQL 物理备份（周日全量，其余增量）
30 1 * * 0 root xtrabackup --backup --target-dir=$BACKUP_BASE/mysql/full-$(date +\%F) && ln -sfn $BACKUP_BASE/mysql/full-$(date +\%F) $BACKUP_BASE/mysql/last
30 1 * * 1-6 root xtrabackup --backup --incremental-basedir=$BACKUP_BASE/mysql/last --target-dir=$BACKUP_BASE/mysql/inc-$(date +\%F) && ln -sfn $BACKUP_BASE/mysql/inc-$(date +\%F) $BACKUP_BASE/mysql/last

# MySQL 逻辑备份（每日）
30 3 * * * root mysqldump --single-transaction --routines --triggers --events --hex-blob --master-data=2 -u backup -p'<PASS>' seeyon \
  | gzip > $BACKUP_BASE/mysql/dump/seeyon_$(date +\%F).sql.gz

# 附件增量备份（每日异机）
30 2 * * * root rsync -a --delete --link-dest=$BACKUP_BASE/files/latest <OA_ATTACH_DIR>/ $BACKUP_BASE/files/snap-$(date +\%F)/ && \
  ln -sfn $BACKUP_BASE/files/snap-$(date +\%F) $BACKUP_BASE/files/latest

# 校验与报告（每日）
30 4 * * * root find $BACKUP_BASE -type f -mtime -1 -print0 | xargs -0 sha256sum > $BACKUP_BASE/checksum/$(date +\%F).sha256 && \
  /usr/local/bin/backup-report --base $BACKUP_BASE --notify wecom
```

> 说明：`$BACKUP_BASE` 与 `<OA_ATTACH_DIR>` 为实际环境路径；建议通过 SSH 密钥/堡垒机到备份主机执行，必要时限速与 QoS。

## 13. 恢复校验脚本片段

```bash
# 从校验清单比对一致性（示例）
sha256sum -c $BACKUP_BASE/checksum/2025-09-17.sha256

# MySQL 恢复后抽检
mysql -uroot -p -e "CHECKSUM TABLE seeyon.formmain_xxx QUICK;"

# 附件抽样校验（随机10个近7日附件是否存在）
shuf -n 10 $BACKUP_BASE/files/latest/filelist.txt | while read f; do [ -f "$f" ] || echo "MISSING: $f"; done
```

## 14. 参数占位与示例

- `<OA_ATTACH_DIR>`：如 `/data/seeyon/att/`
- `<OA_EXPORT_DIR>`：如 `/data/seeyon/export/`
- `<OA_INDEX_DIR>`：如 `/data/seeyon/index/`（可重建）
- `<MYSQL_DATA_DIR>`：如 `/var/lib/mysql`
- `$BACKUP_BASE`（备份主机）：如 `/backup`（挂载到异机盘/iSCSI/NFS）
- `<BACKUP_SERVER>`：如 `10.10.10.50`（仅白名单放行）

## 15. 台账模板

### 15.1 备份任务清单

| 任务名 | 对象 | 频率 | 保留 | 介质 | 加密 | 负责人 |
|---|---|---|---|---|---|---|
| HCI 一致性快照 | 应用+DB VM | 4小时 | 14天 | HCI | - | 存储 |
| MySQL 物理备份 | 数据库 | 全量周/增量日 | 30天 | 异机磁盘 | GPG | DBA |
| MySQL 逻辑备份 | 数据库 | 日 | 30天 | 异机磁盘 | GPG | DBA |
| 附件增量备份 | 附件目录 | 日 | 30天 | 异机磁盘 | - | 应用 |
| 附件服务器快照/复制 | 附件服务器 | 4小时/日结 | 14天/30天 | NAS/HCI | - | 存储 |
| 月度长期留存 | DB+附件 | 月 | 12月+归档 | 对象存储 | WORM | 安全 |

### 15.2 恢复演练记录

| 日期 | 场景 | RTO 实测 | RPO 实测 | 问题与改进 | 负责人 |
|---|---|---|---|---|---|
| 2025-09-10 | DR 启动 | 5h | 20m | 附件目录权限需修正 | 全体 |

### 15.3 变更记录

| 变更项 | 影响资产 | 策略更新点 | 回退方案 | 审批 |
|---|---|---|---|---|
| OA 升级至 Vx.y | 应用/索引 | 备份窗口调整 | 版本回退包 | 单号#xxx |

### 15.4 备件与SLA台账

| 项目 | 厂商 | SLA | 备件位置/数量 | 联系方式 |
|---|---|---|---|---|
| 服务器整机 | Vendor-A | 4h 上门 | 机房A 1套 | 400-XXXX-XXX |
| 磁盘/SSD | Vendor-B | NBD | 机房A 2块 | 400-YYYY-YYY |
| 交换机/光模块 | Vendor-C | NBD | 机房A 若干 | support@example.com |

---

## 16. 落地与下一步

1) 确认实际路径与主机：补齐 `<OA_ATTACH_DIR>`、`$BACKUP_BASE`、`<BACKUP_SERVER>` 等占位
2) 启用/校正 MySQL binlog/GTID 与参数（如尚未）
3) 在深信服 HCI 建立一致性组与复制计划（含应用一致性脚本）
4) 在备份主机部署上述计划任务与校验/报告脚本，首周每日抽检恢复
5) 完成一次端到端演练（含 DR 启动与回切），提交演练报告与台账

## 18. 外部不可抗力与硬件损害应对

### 18.1 定义与场景
- 不可抗力：地震、火灾、洪涝、雷击、社会性停电等导致的基础设施与服务器硬件损坏、机房不可用。

### 18.2 预防性措施
- 备份：严格执行 3-2-1；月度长期副本存放“异地不可变存储”（对象锁/WORM）或离线介质；存储地点与生产物理隔离。
- 容灾：同城/异地 DR 资源预留，HCI 一致性组复制开启并定期演练；关键主机的冷/温备方案。
- 备件与SLA：建立“备件与SLA台账”（见 15.4）；签订硬件维保与到场时限；对关键部件（SSD/网卡/电源）保有最小备件池。
- 通讯与指挥：建立应急通讯录（多通道）、职责矩阵与决策权限；明确停机公告与对外沟通模板。

### 18.3 处置流程（机房级/硬件损毁）
1) 人员安全优先→切断风险源（供电/燃气等）→上报与启动应急预案。
2) 研判影响范围：电力/网络/空调/机柜/硬件；判断恢复时间窗口（ETR）。
3) 决策切换：若 ETR > 2 小时，启动 DR 站点；由存储/HCI 负责人发起一致性组副本“测试启动/正式启动”。
4) DR 启动顺序：数据库→附件→应用→访问入口/DNS（当前为应用直连）。
5) 恢复与验证：按 8 章 Runbook 执行；记录 RTO/RPO 实测与偏差。
6) 硬件维修/重建：并行推进 RMA/更换/采购；恢复生产后制定回切计划与数据差异合并步骤。
7) 复盘与改进：补齐证据、优化演练、补充备件与加固弱点（电力/网络/防火）。

### 18.4 证据与审计
- DR 启动与回切记录、备份校验和、不可变留存截图、SLA 响应记录、供应商工单与到场凭证。

## 17. 附件服务器（独立部署）备份与应急方案

### 17.1 架构与目标
- 架构：附件/正文存放于独立的“附件服务器”（物理机/VM/NAS）。OA 应用通过本地挂载或网络共享（NFS/SMB）访问。
- 目标：在保留“每日异机备份”的同时，利用附件服务器自身快照/复制能力，实现分钟级 RPO（配合数据库/HCI）与小时级 RTO。

### 17.2 备份策略（附件服务器专用）
- 模式选择
  - Push（附件服务器→备份主机）：附件服务器主动推送增量到 `$BACKUP_BASE/files/snap-YYYY-MM-DD`，适合有出网白名单与密钥的场景。
  - Pull（备份主机←附件服务器）：备份主机按计划从附件服务器拉取，适合集中管控/只读账户。
- 通用约束
  - 与数据库时间点一致：在备份窗口短暂静默写入任务，或与 HCI/DB 备份使用相同“冻结/快照”时间点。
  - 不可变与长期留存：月度副本推送至对象存储（对象锁/WORM）。
- Push 示例（在附件服务器执行）
  - 预置：免密 SSH 到备份主机（备份专用账号）、目录 `$BACKUP_BASE/files/`
  - 命令：
    - `RSYNC_BASE=$BACKUP_BASE/files; SRC=<OA_ATTACH_DIR>; DST=$RSYNC_BASE/snap-$(date +%F); rsync -a --delete --link-dest=$RSYNC_BASE/latest "$SRC"/ "$DST"/ && ln -sfn "$DST" $RSYNC_BASE/latest`
- Pull 示例（在备份主机执行）
  - 预置：可读账户或 rsyncd 只读模块；白名单 IP
  - 命令：
    - `RSYNC_BASE=$BACKUP_BASE/files; SRC_USER=backup; SRC_HOST=<ATT_SERVER>; SRC_DIR=<OA_ATTACH_DIR>; DST=$RSYNC_BASE/snap-$(date +%F); rsync -a --delete --link-dest=$RSYNC_BASE/latest $SRC_USER@$SRC_HOST:"$SRC_DIR"/ "$DST"/ && ln -sfn "$DST" $RSYNC_BASE/latest`
- NAS/存储快照（如附件服务器为 NAS）
  - 启用 NAS 原生快照：每 4 小时 1 次 + 每日 1 次日结；保留 14/30 天。
  - 同城/异地复制：对附件共享卷做异步复制，与数据库/应用卷的复制周期对齐。

### 17.3 安全与网络
- 专用 VLAN/VRF 传输；仅白名单主机可访问附件共享/rsync 服务。
- 备份账号最小权限（只读），密钥托管；审计日志开启。
- 勒索防护：对月度留存启用对象锁/WORM；生产共享开启快照不可删除窗口（若支持）。

### 17.4 监控与校验
- 指标：每日新/变更文件数、备份用时与吞吐、失败重试、NAS 复制延迟。
- 校验：生成 `filelist.txt`（`find <OA_ATTACH_DIR> -type f`），抽样比对；对备份目录生成 sha256 校验清单并周度复核。

### 17.5 应急处置（附件服务器相关场景）
- 附件服务器单点故障（应用/DB 正常）
  - 方案A（快速只读）：在应用侧临时挂载 `$BACKUP_BASE/files/latest` 提供只读访问，保障查询；随后回切至修复后的附件服务器。
  - 方案B（恢复替换）：从最近可用快照 `snap-YYYY-MM-DD` 回拷至新卷/新服务器，恢复读写；核对与数据库一致性后开放全量用户。
- 勒索/入侵
  - 立即下线附件共享与网络隔离→切断备份写入→以“不可变月度/异地副本”恢复→全量杀毒/补丁→密钥轮换与溯源。
- 机房级中断（DR 启动）
  - 在 DR 挂载复制到位的附件卷或 `files/snap-YYYY-MM-DD`；按“数据库→附件→应用→入口/DNS”顺序启动；业务联调后公告可写策略；形成回切计划。

### 17.6 配置占位与示例
- `<ATT_SERVER>`：附件服务器主机名/IP（如 `10.10.20.30`）
- `<ATT_PROTOCOL>`：`NFS`/`SMB`/本地挂载（按现网）
- `<ATT_SHARE>`：如 `nfs://nas01:/vol/oa_att` 或 本地路径
- `<OA_ATTACH_DIR>`：如 `/data/seeyon/att`（在附件服务器侧）
- `$BACKUP_BASE`：如 `/backup`（备份主机侧）

### 17.7 示例计划（附件服务器相关）
```cron
# Pull 模式（备份主机）：每日附件增量 + 硬链接快照
30 2 * * * root RSYNC_BASE=$BACKUP_BASE/files; SRC_USER=backup; SRC_HOST=<ATT_SERVER>; SRC_DIR=<OA_ATTACH_DIR>; DST=$RSYNC_BASE/snap-$(date +\%F); \
  rsync -a --delete --link-dest=$RSYNC_BASE/latest $SRC_USER@$SRC_HOST:"$SRC_DIR"/ "$DST"/ && ln -sfn "$DST" $RSYNC_BASE/latest

# Push 模式（附件服务器）：每日附件增量 + 硬链接快照
30 2 * * * backup RSYNC_BASE=$BACKUP_BASE/files; SRC=<OA_ATTACH_DIR>; DST=$RSYNC_BASE/snap-$(date +\%F); \
  rsync -a --delete --link-dest=$RSYNC_BASE/latest "$SRC"/ "$DST"/ && ln -sfn "$DST" $RSYNC_BASE/latest
```
