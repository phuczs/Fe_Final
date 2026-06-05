import {
  Button,
  Input,
  Space,
  Table,
  Tag,
  Tooltip,
  message,
} from 'antd'
import {
  SearchOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  MailOutlined,
  ReloadOutlined,
} from '@ant-design/icons'
import { useEffect, useState } from 'react'

import { emailWhitelistApi } from '../../../api/emailWhitelistApi'
import AddEmailModal from './components/AddEmailModal'
import EditEmailModal from './components/EditEmailModal'
import DeleteEmailModal from './components/DeleteEmailModal'
import './EmailWhitelistPage.css'

function formatDateTime(value) {
  if (!value) return '-'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString()
}

export default function EmailWhitelistPage() {
  const [data, setData] = useState([])
  const [loading, setLoading] = useState(false)
  const [searchValue, setSearchValue] = useState('')

  const [isAddOpen, setIsAddOpen] = useState(false)
  const [editTarget, setEditTarget] = useState(null)
  const [deleteTarget, setDeleteTarget] = useState(null)

  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  })

  // ─── load ─────────────────────────────────────────────────────────────────
  const loadData = async (page = 1, pageSize = 10, query = '') => {
    setLoading(true)
    try {
      const params = { Page: page, PageSize: pageSize }
      if (query) params.Query = query

      const response = await emailWhitelistApi.list(params)

      // Support both paginated { items, totalCount } and plain array responses
      const raw = response.data
      const items = Array.isArray(raw)
        ? raw
        : Array.isArray(raw?.items)
          ? raw.items
          : []

      const total = raw?.totalCount ?? items.length

      setData(
        items.map((item, idx) => ({
          key: item.id ?? idx,
          id: item.id,
          email: item.email ?? item.emailAddress ?? '-',
          note: item.note ?? item.description ?? '-',
          createdAt: formatDateTime(item.createdAt),
          updatedAt: formatDateTime(item.updatedAt),
        })),
      )
      setPagination((prev) => ({ ...prev, current: page, pageSize, total }))
    } catch (err) {
      message.error(
        err.response?.data?.message ||
          err.response?.data?.title ||
          'Unable to load email whitelist.',
      )
      setData([])
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  const handleSearch = (value) => {
    setSearchValue(value)
    loadData(1, pagination.pageSize, value)
  }

  const handleTableChange = (pag) => {
    loadData(pag.current, pag.pageSize, searchValue)
  }

  const handleRefresh = () => {
    loadData(pagination.current, pagination.pageSize, searchValue)
  }

  // ─── columns ──────────────────────────────────────────────────────────────
  const columns = [
    {
      title: '#',
      key: 'index',
      width: 60,
      render: (_, __, index) =>
        (pagination.current - 1) * pagination.pageSize + index + 1,
    },
    {
      title: 'Email Address',
      dataIndex: 'email',
      key: 'email',
      render: (email) => (
        <Space>
          <MailOutlined className="ewl-email-icon" />
          <span className="ewl-email-text">{email}</span>
        </Space>
      ),
    },
    {
      title: 'Note',
      dataIndex: 'note',
      key: 'note',
      render: (note) =>
        note === '-' ? <Tag color="default">—</Tag> : <span>{note}</span>,
    },
    {
      title: 'Created At',
      dataIndex: 'createdAt',
      key: 'createdAt',
    },
    {
      title: 'Updated At',
      dataIndex: 'updatedAt',
      key: 'updatedAt',
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 110,
      align: 'center',
      render: (_, record) => (
        <Space size={4}>
          <Tooltip title="Edit">
            <Button
              type="text"
              size="small"
              icon={<EditOutlined />}
              className="ewl-action-btn ewl-action-btn--edit"
              onClick={() => setEditTarget(record)}
            />
          </Tooltip>
          <Tooltip title="Delete">
            <Button
              type="text"
              size="small"
              icon={<DeleteOutlined />}
              className="ewl-action-btn ewl-action-btn--delete"
              onClick={() => setDeleteTarget(record)}
            />
          </Tooltip>
        </Space>
      ),
    },
  ]

  return (
    <div className="ewl-page">
      {/* ── header ──────────────────────────────────────────────────── */}
      <div className="ewl-header">
        <div className="ewl-header__title-block">
          <h1 className="ewl-header__title">Email Whitelist</h1>
          <p className="ewl-header__subtitle">
            Manage which email addresses are allowed to register in the system.
          </p>
        </div>

        <div className="ewl-header__stat">
          <span className="ewl-stat-value">{pagination.total}</span>
          <span className="ewl-stat-label">whitelisted emails</span>
        </div>
      </div>

      {/* ── toolbar ─────────────────────────────────────────────────── */}
      <div className="ewl-toolbar">
        <Space>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => setIsAddOpen(true)}
            className="ewl-btn-primary"
          >
            Add Email
          </Button>

          <Tooltip title="Refresh">
            <Button
              icon={<ReloadOutlined />}
              onClick={handleRefresh}
              loading={loading}
            />
          </Tooltip>
        </Space>

        <Input
          allowClear
          className="ewl-search"
          placeholder="Search email address..."
          prefix={<SearchOutlined />}
          value={searchValue}
          onChange={(e) => handleSearch(e.target.value)}
        />
      </div>

      {/* ── table ───────────────────────────────────────────────────── */}
      <div className="ewl-table-wrapper">
        <Table
          columns={columns}
          dataSource={data}
          loading={loading}
          pagination={{
            current: pagination.current,
            pageSize: pagination.pageSize,
            total: pagination.total,
            showSizeChanger: true,
            pageSizeOptions: ['10', '20', '50'],
            showTotal: (total) => `Total ${total} entries`,
          }}
          onChange={handleTableChange}
          locale={{ emptyText: 'No whitelisted emails found.' }}
          scroll={{ x: 700 }}
        />
      </div>

      {/* ── modals ──────────────────────────────────────────────────── */}
      <AddEmailModal
        open={isAddOpen}
        onClose={() => setIsAddOpen(false)}
        onSuccess={() => loadData(1, pagination.pageSize, searchValue)}
      />

      <EditEmailModal
        open={!!editTarget}
        record={editTarget}
        onClose={() => setEditTarget(null)}
        onSuccess={() =>
          loadData(pagination.current, pagination.pageSize, searchValue)
        }
      />

      <DeleteEmailModal
        open={!!deleteTarget}
        record={deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onSuccess={() =>
          loadData(pagination.current, pagination.pageSize, searchValue)
        }
      />
    </div>
  )
}
