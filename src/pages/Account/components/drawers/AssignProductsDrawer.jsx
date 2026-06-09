import { useEffect, useState } from 'react'
import {
  Alert,
  Form,
  TreeSelect,
  Typography,
  message,
} from 'antd'
import { AppstoreOutlined } from '@ant-design/icons'

import BaseDrawer from '../../../../components/common/BaseDrawer'
import { usersApi } from '../../../../api/usersApi'
import {
  PRODUCT_KEY_TO_ID,
  PRODUCT_ID_TO_KEY,
  PRODUCT_TREE_DATA,
} from '../../../../constants/products'

const { Text } = Typography

/**
 * Converts an array of integer productIds from the backend
 * to the tree-node string values used by the TreeSelect.
 */
function idsToTreeValues(productIds = []) {
  return productIds
    .map((id) => PRODUCT_ID_TO_KEY[id])
    .filter(Boolean)
}

/**
 * Converts selected tree-node values (including parent nodes)
 * back to integer productIds for the backend.
 */
function treeValuesToIds(values = []) {
  const ids = []
  values.forEach((val) => {
    // skip parent-group nodes
    if (val === 'curricula' || val === 'vitae') return
    const id = PRODUCT_KEY_TO_ID[val]
    if (id !== undefined) ids.push(id)
  })
  return ids
}

/**
 * AssignProductsDrawer
 *
 * Props:
 *   open        – boolean
 *   onClose     – () => void
 *   onSuccess   – () => void  (called after a successful save)
 *   user        – { id, displayName, emailAddress, productIds: number[] }
 */
export default function AssignProductsDrawer({
  open,
  onClose,
  onSuccess,
  user,
}) {
  const [form] = Form.useForm()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  // Pre-fill the form whenever the selected user changes
  useEffect(() => {
    if (open && user) {
      form.setFieldsValue({
        products: idsToTreeValues(user.productIds),
      })
      setError('')
    }
  }, [open, user, form])

  const handleSubmit = async (values) => {
    setLoading(true)
    setError('')

    try {
      const productIds = treeValuesToIds(values.products || [])

      await usersApi.assignProducts(user.id, { productIds })

      message.success(`Products assigned to "${user.displayName}" successfully.`)
      if (onSuccess) onSuccess()
      onClose()
    } catch (err) {
      const msg =
        err.response?.data?.message ||
        err.response?.data?.title ||
        'Failed to assign products. Please try again.'
      setError(msg)
    } finally {
      setLoading(false)
    }
  }

  return (
    <BaseDrawer
      title="Assign Products"
      width={520}
      open={open}
      onClose={onClose}
      showFooter
      confirmText="Save"
      confirmLoading={loading}
      onConfirm={() => form.submit()}
    >
      {/* User context */}
      {user && (
        <div
          style={{
            marginBottom: 24,
            padding: '12px 16px',
            background: '#f0f5ff',
            border: '1px solid #adc6ff',
            borderRadius: 8,
            display: 'flex',
            alignItems: 'center',
            gap: 12,
          }}
        >
          <AppstoreOutlined style={{ fontSize: 20, color: '#2f54eb' }} />
          <div>
            <Text strong style={{ display: 'block' }}>
              {user.displayName}
            </Text>
            <Text type="secondary" style={{ fontSize: 12 }}>
              {user.emailAddress}
            </Text>
          </div>
        </div>
      )}

      {error && (
        <Alert
          type="error"
          message={error}
          showIcon
          style={{ marginBottom: 16 }}
        />
      )}

      <Form form={form} layout="vertical" onFinish={handleSubmit}>
        <Form.Item
          label="Products"
          name="products"
          rules={[
            {
              required: true,
              message: 'Please select at least one product.',
            },
          ]}
        >
          <TreeSelect
            treeData={PRODUCT_TREE_DATA}
            treeCheckable
            showCheckedStrategy={TreeSelect.SHOW_CHILD}
            placeholder="Select products to assign"
            style={{ width: '100%' }}
            maxTagCount={5}
            treeDefaultExpandAll
          />
        </Form.Item>
      </Form>
    </BaseDrawer>
  )
}
