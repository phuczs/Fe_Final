import {
  Checkbox,
  Collapse,
  Form,
  Space,
} from 'antd'
import { useState } from 'react'

import BaseDrawer from '../../../../components/common/BaseDrawer'
import './FilterDrawer.css'

const roleMap = {
  'tenant-user': 0,
  'system-admin': 1,
}

const statusMap = {
  active: 0,
  inactive: 1,
}

const signInMethodMap = {
  local: 0,
  singpass: 1,
}

export default function FilterDrawer({
  open,
  onClose,
  onApplyFilter,
}) {
  const [form] = Form.useForm()
  const [checkboxValues, setCheckboxValues] = useState({
    role: [],
    status: [],
    signInMethod: [],
  })

  const handleApply = () => {
    const params = {}

    if (checkboxValues.role && checkboxValues.role.length > 0) {
      params.Role = checkboxValues.role.map((r) => roleMap[r]).join(',')
    }

    if (checkboxValues.status && checkboxValues.status.length > 0) {
      params.Status = checkboxValues.status.map((s) => statusMap[s]).join(',')
    }

    if (checkboxValues.signInMethod && checkboxValues.signInMethod.length > 0) {
      params.SignInMethod = checkboxValues.signInMethod.map((m) => signInMethodMap[m]).join(',')
    }

    onApplyFilter?.(params)
    onClose()
  }

  const handleClear = () => {
    setCheckboxValues({
      role: [],
      status: [],
      signInMethod: [],
    })
    form.resetFields()
  }

  const items = [
    {
      key: 'role',
      label: 'Role',
      children: (
        <Space orientation="vertical">
          <Checkbox
            value="system-admin"
            checked={checkboxValues.role.includes('system-admin')}
            onChange={(e) => {
              const newRoles = e.target.checked
                ? [...checkboxValues.role, 'system-admin']
                : checkboxValues.role.filter((r) => r !== 'system-admin')
              setCheckboxValues({ ...checkboxValues, role: newRoles })
            }}
          >
            System admin
          </Checkbox>

          <Checkbox
            value="tenant-user"
            checked={checkboxValues.role.includes('tenant-user')}
            onChange={(e) => {
              const newRoles = e.target.checked
                ? [...checkboxValues.role, 'tenant-user']
                : checkboxValues.role.filter((r) => r !== 'tenant-user')
              setCheckboxValues({ ...checkboxValues, role: newRoles })
            }}
          >
            Tenant user
          </Checkbox>
        </Space>
      ),
    },
    {
      key: 'status',
      label: 'Status',
      children: (
        <Space orientation="vertical">
          <Checkbox
            value="active"
            checked={checkboxValues.status.includes('active')}
            onChange={(e) => {
              const newStatus = e.target.checked
                ? [...checkboxValues.status, 'active']
                : checkboxValues.status.filter((s) => s !== 'active')
              setCheckboxValues({ ...checkboxValues, status: newStatus })
            }}
          >
            Active
          </Checkbox>

          <Checkbox
            value="inactive"
            checked={checkboxValues.status.includes('inactive')}
            onChange={(e) => {
              const newStatus = e.target.checked
                ? [...checkboxValues.status, 'inactive']
                : checkboxValues.status.filter((s) => s !== 'inactive')
              setCheckboxValues({ ...checkboxValues, status: newStatus })
            }}
          >
            Inactive
          </Checkbox>
        </Space>
      ),
    },
    {
      key: 'signInMethod',
      label: 'Sign-in method',
      children: (
        <Space orientation="vertical">
          <Checkbox
            value="local"
            checked={checkboxValues.signInMethod.includes('local')}
            onChange={(e) => {
              const newMethods = e.target.checked
                ? [...checkboxValues.signInMethod, 'local']
                : checkboxValues.signInMethod.filter((m) => m !== 'local')
              setCheckboxValues({ ...checkboxValues, signInMethod: newMethods })
            }}
          >
            Local user
          </Checkbox>

          <Checkbox
            value="singpass"
            checked={checkboxValues.signInMethod.includes('singpass')}
            onChange={(e) => {
              const newMethods = e.target.checked
                ? [...checkboxValues.signInMethod, 'singpass']
                : checkboxValues.signInMethod.filter((m) => m !== 'singpass')
              setCheckboxValues({ ...checkboxValues, signInMethod: newMethods })
            }}
          >
            Singpass user
          </Checkbox>
        </Space>
      ),
    },
  ]

  return (
    <BaseDrawer
      title="Filters"
      size="large"
      open={open}
      onClose={onClose}
      confirmText="Apply"
      cancelText="Clear All"
      showFooter
      onConfirm={handleApply}
      onCancel={handleClear}
    >
      <Form form={form} layout="vertical">
        <Collapse
          className="filter-collapse"
          defaultActiveKey={['role', 'status', 'signInMethod']}
          items={items}
        />
      </Form>
    </BaseDrawer>
  )
}