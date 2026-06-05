import { Checkbox, Input, Button } from 'antd'
import { SearchOutlined } from '@ant-design/icons'
import { useState } from 'react'
import './ColumnSettingsPopup.css'

const defaultItems = [
  { key: 'id', label: 'User ID' },
  { key: 'displayName', label: 'Display Name' },
  { key: 'userId', label: 'Login ID' },
  { key: 'tenantId', label: 'Tenant ID' },
  { key: 'emailAddress', label: 'Email Address' },
  { key: 'staffStudentId', label: 'Staff/Student ID' },
  { key: 'sex', label: 'Sex' },
  { key: 'mobilePhone', label: 'Mobile Phone' },
  { key: 'role', label: 'Role' },
  { key: 'signInMethod', label: 'Sign-in Method' },
  { key: 'productIds', label: 'Product IDs' },
  { key: 'mfaEnabled', label: 'MFA Enabled' },
  { key: 'status', label: 'Status' },
  { key: 'lastLoginAt', label: 'Last Login At' },
  { key: 'createdAt', label: 'Created At' },
  { key: 'updatedAt', label: 'Updated At' },
]

export default function ColumnSettingsPopup({
  value = [],
  onChange,
  onClose,
}) {
  const [search, setSearch] = useState('')

  const filteredItems = defaultItems.filter((i) =>
    i.label.toLowerCase().includes(search.toLowerCase()),
  )

  const allChecked = filteredItems.every((i) =>
    value.includes(i.key),
  )

  const toggleAll = (checked) => {
    if (checked) {
      onChange(filteredItems.map((i) => i.key))
    } else {
      onChange([])
    }
  }

  const toggleItem = (key) => {
    if (value.includes(key)) {
      onChange(value.filter((v) => v !== key))
    } else {
      onChange([...value, key])
    }
  }

  return (
    <div className="column-popup">
      {/* SEARCH */}
      <div className="column-popup__search">
        <Input
          placeholder="Input keyword"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          prefix={<SearchOutlined />}
        />
      </div>

      {/* SELECT ALL */}
      <div className="column-popup__item column-popup__select-all">
        <Checkbox
          checked={allChecked}
          onChange={(e) => toggleAll(e.target.checked)}
        >
          Select all
        </Checkbox>
      </div>

      {/* LIST */}
      <div className="column-popup__list">
        {filteredItems.map((item) => (
          <div key={item.key} className="column-popup__item">
            <Checkbox
              checked={value.includes(item.key)}
              onChange={() => toggleItem(item.key)}
            >
              {item.label}
            </Checkbox>
          </div>
        ))}
      </div>

      {/* FOOTER */}
      <div className="column-popup__footer">
        <Button onClick={() => onChange([])}>Reset</Button>

        <div className="column-popup__footer-right">
          <Button onClick={onClose}>Cancel</Button>
          <Button type="primary" onClick={onClose}>
            OK
          </Button>
        </div>
      </div>
    </div>
  )
}