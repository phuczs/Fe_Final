import {
  Card,
  Input,
  Row,
  Col,
  Select,
  Table,
  Tag,
} from 'antd'
import { useEffect, useState } from 'react'

import {
  SearchOutlined,
  DownloadOutlined,
} from '@ant-design/icons'

import { auditApi } from '../../../api/auditApi'
import PrimaryButton from '../../../components/common/PrimaryButton'
import SecondaryButton from '../../../components/common/SecondaryButton'

const columns = [
  {
    title: 'ID',
    dataIndex: 'id',
    key: 'id',
  },
  {
    title: 'Actor Email',
    dataIndex: 'actorEmail',
    key: 'actorEmail',
  },
  {
    title: 'Object Type',
    dataIndex: 'objectType',
    key: 'objectType',
    render: (value) => <Tag color="blue">{value}</Tag>,
  },
  {
    title: 'Action',
    dataIndex: 'action',
    key: 'action',
  },
  {
    title: 'Target Name',
    dataIndex: 'targetName',
    key: 'targetName',
  },
  {
    title: 'Target User ID',
    dataIndex: 'targetUserId',
    key: 'targetUserId',
  },
  {
    title: 'Change Detail',
    dataIndex: 'changeDetail',
    key: 'changeDetail',
    render: (value) => {
      if (!value || value === '-') return '-'

      let detail = null

      try {
        detail = JSON.parse(value)
      } catch {
        detail = null
      }

      if (!detail || typeof detail !== 'object') {
        return value
      }

      const entries = Object.entries(detail)

      return (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
          <div>
            <Tag color={detail.Result === 'Success' ? 'success' : 'error'}>
              {detail.Result || 'Detail'}
            </Tag>
          </div>

          <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6 }}>
            {entries
              .filter(([key]) => key !== 'Result')
              .map(([key, itemValue]) => (
                <Tag key={key} color="default">
                  {key}: {String(itemValue)}
                </Tag>
              ))}
          </div>
        </div>
      )
    },
  },
  {
    title: 'Occurred At',
    dataIndex: 'occurredAt',
    key: 'occurredAt',
  },
]

function formatDateTime(value) {
  if (!value) return '-'

  const date = new Date(value)

  if (Number.isNaN(date.getTime())) return value

  return date.toLocaleString()
}

function normalizeText(value) {
  if (value === null || value === undefined || value === '') return '-'

  return String(value)
} 

function mapAuditToRows(items = []) {
  return items.map((item) => ({
    key: item.id,
    id: item.id,
    actorEmail: normalizeText(item.actorEmail),
    objectType: normalizeText(item.objectType),
    action: normalizeText(item.action),
    targetName: normalizeText(item.targetName),
    targetUserId: normalizeText(item.targetUserId),
    changeDetail: normalizeText(item.changeDetail),
    occurredAt: formatDateTime(item.occurredAt),
  }))
}

export default function UserActivityReportPage() {
  const [loading, setLoading] = useState(false)
  const [exportLoading, setExportLoading] = useState(false)
  const [data, setData] = useState([])
  const [error, setError] = useState('')
  const [filters, setFilters] = useState({
    Search: '',
    ObjectType: 'User',
    TargetName: '',
    TargetUserId: '',
    SortDirection: 'desc',
  })
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  })

  const [searchValue, setSearchValue] = useState('')
  const [objectType, setObjectType] = useState('User')
  const [targetName, setTargetName] = useState('')
  const [targetUserId, setTargetUserId] = useState('')
  const [sortDirection, setSortDirection] = useState('desc')

  const buildRequestFilters = () => {
    const nextFilters = {}

    if (searchValue.trim()) nextFilters.Search = searchValue.trim()
    if (objectType && objectType !== 'All') nextFilters.ObjectType = objectType
    if (targetName.trim()) nextFilters.TargetName = targetName.trim()
    if (targetUserId.trim()) nextFilters.TargetUserId = targetUserId.trim()
    if (sortDirection) nextFilters.SortDirection = sortDirection

    return nextFilters
  }

  const loadAuditLogs = async (requestFilters = {}, page = 1, pageSize = 10) => {
    setLoading(true)
    setError('')

    try {
      const params = {
        Page: page,
        PageSize: pageSize,
        ...requestFilters,
      }

      const response = await auditApi.list(params)
      setData(mapAuditToRows(response.data?.items || []))
      setPagination({
        current: response.data?.page || page,
        pageSize: response.data?.pageSize || pageSize,
        total: response.data?.totalCount || 0,
      })
    } catch (requestError) {
      setError(
        requestError.response?.data?.message ||
        requestError.response?.data?.title ||
        'Unable to load audit logs at this time.',
      )
      setData([])
    } finally {
      setLoading(false)
    }
  }

  const handleSearch = () => {
    const nextFilters = buildRequestFilters()

    setFilters(nextFilters)
    loadAuditLogs(nextFilters, 1, pagination.pageSize)
  }

  const handleExport = async () => {
    const nextFilters = buildRequestFilters()

    setExportLoading(true)

    try {
      const response = await auditApi.export(nextFilters)
      const blob = new Blob([response.data], {
        type: response.headers['content-type'] || 'application/octet-stream',
      })
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')

      link.href = url
      link.download = 'audit-report'
      document.body.appendChild(link)
      link.click()
      link.remove()
      window.URL.revokeObjectURL(url)
    } catch (requestError) {
      setError(
        requestError.response?.data?.message ||
        requestError.response?.data?.title ||
        'Unable to export audit logs at this time.',
      )
    } finally {
      setExportLoading(false)
    }
  }

  useEffect(() => {
    const initialFilters = {
      ObjectType: 'User',
      SortDirection: 'desc',
    }

    setFilters(initialFilters)
    loadAuditLogs(initialFilters)
  }, [])

  return (
    <div
      style={{
        display: 'flex',
        flexDirection: 'column',
        gap: 24,
      }}
    >
      <Card>
        <Row gutter={[16, 16]}>
          <Col span={8}>
            <label>Search</label>

            <Input
              placeholder="Search audit logs..."
              value={searchValue}
              onChange={(e) => setSearchValue(e.target.value)}
              onPressEnter={handleSearch}
            />
          </Col>

          <Col span={8}>
            <label>Object Type</label>

            <Select
              style={{ width: '100%' }}
              value={objectType}
              onChange={setObjectType}
              options={[
                { value: 'User', label: 'User' },
                { value: 'All', label: 'All' },
              ]}
            />
          </Col>

          <Col span={8}>
            <label>Target Name</label>

            <Input
              placeholder="Search target name..."
              value={targetName}
              onChange={(e) => setTargetName(e.target.value)}
              onPressEnter={handleSearch}
            />
          </Col>

          <Col span={8}>
            <label>Target User ID</label>

            <Input
              placeholder="Search target user id..."
              value={targetUserId}
              onChange={(e) => setTargetUserId(e.target.value)}
              onPressEnter={handleSearch}
            />
          </Col>

          <Col span={8}>
            <label>Sort Direction</label>

            <Select
              style={{ width: '100%' }}
              value={sortDirection}
              onChange={setSortDirection}
              options={[
                { value: 'desc', label: 'Descending' },
                { value: 'asc', label: 'Ascending' },
              ]}
            />
          </Col>
        </Row>

        <div
          style={{
            marginTop: 24,
            display: 'flex',
            justifyContent: 'flex-end',
            gap: 8,
          }}
        >
          <SecondaryButton icon={<DownloadOutlined />} onClick={handleExport} loading={exportLoading}>
            Export
          </SecondaryButton>

          <PrimaryButton icon={<SearchOutlined />} onClick={handleSearch}>
            Search
          </PrimaryButton>
        </div>
      </Card>

      <Card>
        <Table
          rowKey="id"
          columns={columns}
          dataSource={data}
          loading={loading}
          locale={{ emptyText: error || 'No audit logs found.' }}
          pagination={{
            current: pagination.current,
            pageSize: pagination.pageSize,
            total: pagination.total,
            showSizeChanger: true,
            pageSizeOptions: ['10', '20', '50', '100'],
            onChange: (page, pageSize) => {
              loadAuditLogs(filters, page, pageSize)
            },
          }}
        />
      </Card>
    </div>
  )
}