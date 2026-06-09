import {
  Alert,
  Button,
  Input,
  Space,
  Table,
  Tabs,
  Tag,
} from 'antd'
import { useEffect, useState } from 'react'

import {
  SearchOutlined,
  UploadOutlined,
  DownloadOutlined,
  UsergroupAddOutlined,
  FilterFilled,
  SettingFilled,
  EllipsisOutlined,
} from '@ant-design/icons'

import { usersApi } from '../../api/usersApi'
import ActivateUsersModal from '../../components/common/ActivateUsersModal'
import DeactivateUsersModal from '../../components/common/DeactivateUsersModal'
import DeleteUsersModal from '../../components/common/DeleteUsersModal'
import AddUserDrawer from './components/drawers/AddUserDrawer'
import FilterDrawer from './components/drawers/FilterDrawer'
import ColumnSettingsPopup from './components/settings/ColumnSettingsPopup'
import AssignProductsDrawer from './components/drawers/AssignProductsDrawer'
import { PRODUCT_LABEL_MAP, PRODUCT_TAG_COLORS } from '../../constants/products'
import './AccountPage.css'

const roleLabels = {
  0: 'Tenant User',
  1: 'System Admin',
}

const statusLabels = {
  0: 'Active',
  1: 'Deactivated',
}

const signInMethodLabels = {
  0: 'Local user',
  1: 'Singpass user',
}

const sexLabels = {
  male: 'Male',
  female: 'Female',
}

const sexTagColors = {
  Male: 'processing',
  Female: 'magenta',
}

const roleTagColors = {
  'System Admin': 'gold',
  'Tenant User': 'geekblue',
}

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

function mapUsersToRows(items = []) {
  return items.map((user) => ({
    key: user.id,
    id: normalizeText(user.id),
    displayName: normalizeText(user.displayName),
    userId: normalizeText(user.userId),
    tenantId: normalizeText(user.tenantId),
    emailAddress: normalizeText(user.emailAddress),
    staffStudentId: normalizeText(user.staffStudentId),
    sex: sexLabels[user.sex ?? 'male'] || normalizeText(user.sex ?? 'male'),
    mobilePhone: normalizeText(user.mobilePhone),
    role: roleLabels[user.role] || `Role ${user.role ?? ''}`.trim(),
    signInMethod: signInMethodLabels[user.signInMethod] || `Method ${user.signInMethod ?? ''}`.trim(),
    productIds: Array.isArray(user.productIds) ? user.productIds : [],
    mfaEnabled: user.mfaEnabled ? 'Yes' : 'No',
    status: statusLabels[user.status] || `Status ${user.status ?? ''}`.trim(),
    lastLoginAt: formatDateTime(user.lastLoginAt),
    createdAt: formatDateTime(user.createdAt),
    updatedAt: formatDateTime(user.updatedAt),
  }))
}

const columns = [
  {
    title: 'User ID',
    dataIndex: 'id',
    key: 'id',
  },
  {
    title: 'Display Name',
    dataIndex: 'displayName',
    key: 'displayName',
  },
  {
    title: 'Login ID',
    dataIndex: 'userId',
    key: 'userId',
  },
  {
    title: 'Tenant ID',
    dataIndex: 'tenantId',
    key: 'tenantId',
  },
  {
    title: 'Email Address',
    dataIndex: 'emailAddress',
    key: 'emailAddress',
  },
  {
    title: 'Staff/Student ID',
    dataIndex: 'staffStudentId',
    key: 'staffStudentId',
  },
  {
    title: 'Sex',
    dataIndex: 'sex',
    key: 'sex',
    render: (sex) => (
      <Tag color={sexTagColors[sex] || 'default'}>
        {sex}
      </Tag>
    ),
  },
  {
    title: 'Mobile Phone',
    dataIndex: 'mobilePhone',
    key: 'mobilePhone',
  },
  {
    title: 'Role',
    dataIndex: 'role',
    key: 'role',
    render: (role) => (
      <Tag color={roleTagColors[role] || 'default'}>
        {role}
      </Tag>
    ),
  },
  {
    title: 'Sign-in Method',
    dataIndex: 'signInMethod',
    key: 'signInMethod',
  },
  {
    title: 'Assigned Products',
    dataIndex: 'productIds',
    key: 'productIds',
    width: 280,
    render: (productIds) => {
      if (!Array.isArray(productIds) || productIds.length === 0) {
        return <Tag color="default">None</Tag>
      }
      return (
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: 4 }}>
          {productIds.map((id) => (
            <Tag key={id} color={PRODUCT_TAG_COLORS[id] || 'default'}>
              {PRODUCT_LABEL_MAP[id] || `Product ${id}`}
            </Tag>
          ))}
        </div>
      )
    },
  },
  {
    title: 'MFA Enabled',
    dataIndex: 'mfaEnabled',
    key: 'mfaEnabled',
  },
  {
    title: 'Status',
    dataIndex: 'status',
    key: 'status',
    render: (status) =>
      status === 'Active' ? (
        <Tag color="success">Active</Tag>
      ) : (
        <Tag color="error">Deactivated</Tag>
      ),
  },
  {
    title: 'Last Login At',
    dataIndex: 'lastLoginAt',
    key: 'lastLoginAt',
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
]

const allColumnKeys = columns.map((col) => col.key)

export default function AccountPage() {
  const [isAddUserDrawerOpen, setIsAddUserDrawerOpen] = useState(false)
  const [isFilterDrawerOpen, setIsFilterDrawerOpen] = useState(false)
  const [isColumnSettingsOpen, setIsColumnSettingsOpen] = useState(false)
  const [selectedColumns, setSelectedColumns] = useState(allColumnKeys)
  const [loadingUsers, setLoadingUsers] = useState(false)
  const [usersError, setUsersError] = useState('')
  const [data, setData] = useState([])
  const [rawUsers, setRawUsers] = useState([])  // raw API items for lookups
  const [filters, setFilters] = useState({})
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
    totalPages: 0,
  })

  const [selectedRowKeys, setSelectedRowKeys] = useState([])
  const [isActivateModalOpen, setIsActivateModalOpen] = useState(false)
  const [isDeactivateModalOpen, setIsDeactivateModalOpen] = useState(false)
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false)
  const [isAssignProductsOpen, setIsAssignProductsOpen] = useState(false)
  const [searchValue, setSearchValue] = useState('')

  const loadUsers = async (filterParams = {}, pageNum = 1, pageSize = 10) => {
    setLoadingUsers(true)
    setUsersError('')

    try {
      const params = {
        Page: pageNum,
        PageSize: pageSize,
        ...filterParams,
      }

      const response = await usersApi.list(params)
      const items = response.data?.items || []
      setRawUsers(items)
      setData(mapUsersToRows(items))
      setPagination({
        current: response.data?.page || pageNum,
        pageSize: response.data?.pageSize || pageSize,
        total: response.data?.totalCount || 0,
        totalPages: response.data?.totalPages || 1,
      })
    } catch (requestError) {
      setUsersError(
        requestError.response?.data?.message ||
        requestError.response?.data?.title ||
        'Unable to load users at this time.',
      )
      setData([])
    } finally {
      setLoadingUsers(false)
    }
  }

  const handleApplyFilter = (filterValues) => {
    const params = {}

    if (filterValues.search) params.Query = filterValues.search
    if (filterValues.Role) params.Role = filterValues.Role
    if (filterValues.Status) params.Status = filterValues.Status
    if (filterValues.SignInMethod) params.SignInMethod = filterValues.SignInMethod
    if (filterValues.ProductId) params.ProductId = filterValues.ProductId

    setFilters(filterValues)
    loadUsers(params, 1, pagination.pageSize)
  }

  const handleSearch = (value) => {
    setSearchValue(value)
    const newFilters = { ...filters, search: value }
    const params = {}
    if (value) params.Query = value
    if (newFilters.Role) params.Role = newFilters.Role
    if (newFilters.Status) params.Status = newFilters.Status
    if (newFilters.SignInMethod) params.SignInMethod = newFilters.SignInMethod
    if (newFilters.ProductId) params.ProductId = newFilters.ProductId

    loadUsers(params, 1, pagination.pageSize)
  }

  useEffect(() => {
    loadUsers()

    // Debug: fetch and print database products to console
    import('../../api/productsApi').then(({ productsApi }) => {
      productsApi.getAllActive()
        .then(res => {
          console.log('DEBUG [DB Products]:', res.data)
        })
        .catch(err => {
          console.error('DEBUG [DB Products Error]:', err)
        })
    })
  }, [])

  const rowSelection = {
    selectedRowKeys,
    onChange: (keys) => {
      setSelectedRowKeys(keys)
    },
  }

  const handleDeactivateSuccess = async () => {
    setSelectedRowKeys([])
    await loadUsers()
  }

  const visibleColumns = columns.filter((col) =>
    selectedColumns.includes(col.key),
  )

  return (
    <>
      <div
        className="account-page"
        style={{
          display: 'flex',
          flexDirection: 'column',
          gap: 20,
          width: '100%',
          minWidth: 0,
          overflow: 'hidden',
        }}
      >
        <Alert
          title="Your tenant has enabled the allow list, which is preventing notifications from being sent to users."
          type="info"
          showIcon
        />

        <Tabs
          defaultActiveKey="users"
          items={[
            { key: 'users', label: 'User' },
            { key: 'company', label: 'Company' },
          ]}
        />

        <div
          style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',

            flexWrap: 'nowrap',
            gap: 12,
            width: '100%',
          }}
        >
          {/* LEFT ACTIONS */}
          <Space style={{ whiteSpace: 'nowrap' }}>
            <Button
              type="primary"
              icon={<UsergroupAddOutlined />}
              onClick={() => setIsAddUserDrawerOpen(true)}
            >
              Add Users/Groups
            </Button>

            <Button icon={<DownloadOutlined />}>Export</Button>
            <Button icon={<UploadOutlined />}>Import</Button>
            <Button>Manage Admin Roles</Button>

            {selectedRowKeys.length > 0 && (
              <>
                <Button onClick={() => setIsActivateModalOpen(true)}>
                  Activate
                </Button>

                <Button danger onClick={() => setIsDeactivateModalOpen(true)}>
                  Deactivate
                </Button>

                <Button danger onClick={() => setIsDeleteModalOpen(true)}>
                  Delete
                </Button>

                {/* Assign Products — only for a single Tenant User selection */}
                {selectedRowKeys.length === 1 && (() => {
                  const raw = rawUsers.find((u) => u.id === selectedRowKeys[0])
                  return raw?.role === 0 ? (
                    <Button
                      type="primary"
                      ghost
                      onClick={() => setIsAssignProductsOpen(true)}
                    >
                      Assign Products
                    </Button>
                  ) : null
                })()}
              </>
            )}
          </Space>

          <div
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: 8,
              whiteSpace: 'nowrap',
              position: 'relative',
            }}
          >
            <Button type="text" icon={<EllipsisOutlined />} />
            <Button
              type="text"
              icon={<SettingFilled />}
              onClick={() => setIsColumnSettingsOpen(!isColumnSettingsOpen)}
            />

            {isColumnSettingsOpen && (
              <div style={{ position: 'absolute', left: 0, top: 40, zIndex: 1000 }}>
                <ColumnSettingsPopup
                  value={selectedColumns}
                  onChange={setSelectedColumns}
                  onClose={() => setIsColumnSettingsOpen(false)}
                />
              </div>
            )}

            <Button
              type="text"
              icon={<FilterFilled />}
              onClick={() => setIsFilterDrawerOpen(true)}
            />

            <Input
              allowClear
              style={{ width: 320, flexShrink: 0 }}
              placeholder="Search by display name, user ID..."
              prefix={<SearchOutlined />}
              value={searchValue}
              onChange={(e) => handleSearch(e.target.value)}
            />
          </div>
        </div>

        <div
          style={{
            width: '100%',
            overflowX: 'auto',
            overflowY: 'hidden',
          }}
        >
          <Table
            rowSelection={rowSelection}
            columns={visibleColumns}
            dataSource={data}
            loading={loadingUsers}
            locale={{ emptyText: usersError || 'No users found.' }}
            pagination={{
              current: pagination.current,
              pageSize: pagination.pageSize,
              total: pagination.total,
              onChange: (page, pageSize) => {
                loadUsers(filters, page, pageSize)
              },
              showSizeChanger: true,
              pageSizeOptions: ['10', '20', '50', '100'],
            }}
            scroll={{ x: 1200 }}
          />
        </div>
      </div>

      {/* DRAWERS */}
      <AddUserDrawer
        open={isAddUserDrawerOpen}
        onClose={() => setIsAddUserDrawerOpen(false)}
        onSuccess={() => {
          // Reload table data starting from page 1 when a new user is added
          loadUsers(filters, 1, pagination.pageSize)
        }}
      />

      <FilterDrawer
        open={isFilterDrawerOpen}
        onClose={() => setIsFilterDrawerOpen(false)}
        onApplyFilter={handleApplyFilter}
      />

      {/* MODALS */}
      <ActivateUsersModal
        open={isActivateModalOpen}
        onClose={() => setIsActivateModalOpen(false)}
        selectedUserIds={selectedRowKeys}
        onSuccess={handleDeactivateSuccess}
      />

      <DeactivateUsersModal
        open={isDeactivateModalOpen}
        onClose={() => setIsDeactivateModalOpen(false)}
        selectedUserIds={selectedRowKeys}
        onSuccess={handleDeactivateSuccess}
      />

      <DeleteUsersModal
        open={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        selectedUserIds={selectedRowKeys}
        onSuccess={handleDeactivateSuccess}
      />

      <AssignProductsDrawer
        open={isAssignProductsOpen}
        onClose={() => setIsAssignProductsOpen(false)}
        onSuccess={() => {
          setSelectedRowKeys([])
          loadUsers(filters, pagination.current, pagination.pageSize)
        }}
        user={rawUsers.find((u) => u.id === selectedRowKeys[0]) ?? null}
      />
    </>
  )
}