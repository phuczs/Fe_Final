import { useState } from 'react'
import {
  Form,
  Input,
  Radio,
  Space,
  TreeSelect,
  message,
  Modal,      
  Typography  
} from 'antd'

import BaseDrawer from '../../../../components/common/BaseDrawer'
import { usersApi } from '../../../../api/usersApi'

const { Paragraph } = Typography 

// BẢNG QUY ĐỔI TỪ CHỮ SANG SỐ NGUYÊN
const PRODUCT_ID_MAP = {
  'curricula-student': 1,
  'curricula-staff': 2,
  'vitae-student': 3,
  'vitae-staff': 4,
  'vitae-trainer': 5
}

// TreeData mặc định để hiển thị Giao diện
const productTreeData = [
  {
    title: 'Curricula for Training',
    value: 'curricula',
    key: 'curricula',
    children: [
      { title: 'Student', value: 'curricula-student', key: 'curricula-student' },
      { title: 'Staff', value: 'curricula-staff', key: 'curricula-staff' },
    ],
  },
  {
    title: 'Vitae',
    value: 'vitae',
    key: 'vitae',
    children: [
      { title: 'Student', value: 'vitae-student', key: 'vitae-student' },
      { title: 'Staff', value: 'vitae-staff', key: 'vitae-staff' },
      { title: 'Trainer', value: 'vitae-trainer', key: 'vitae-trainer' },
    ],
  },
]

export default function AddUserDrawer({ open, onClose, onSuccess }) {
  const [form] = Form.useForm()
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (values) => {
    setLoading(true)

    try {
      // 1. QUY ĐỔI PRODUCT CHỮ -> SỐ NGUYÊN
      const rawProducts = values.product || []
      const safeProductIds = []

      rawProducts.forEach(val => {
        if (val === 'curricula' || val === 'vitae') return
        const mappedId = PRODUCT_ID_MAP[val]
        if (mappedId !== undefined) {
          safeProductIds.push(mappedId)
        }
      })

      // 2. TẠO PAYLOAD
      const payload = {
        users: [
          {
            userId: values.userId || "",
            emailAddress: values.email || "", 
            displayName: values.displayName || "",
            staffStudentId: values.staffId || "", 
            sex: values.sex === 'female' ? 'Female' : 'Male',
            mobilePhone: values.mobilePhone || "",
            role: values.role === 'tenant-admin' ? 1 : 0,
            signInMethod: values.signInMethod === 'singpass' ? 1 : 0,
            productIds: safeProductIds, 
          }
        ]
      }

      // 3. GỌI API
      const response = await usersApi.create(payload)
      const responseData = response.data

      // 4. LẤY TEMPORARY PASSWORD
      // Bao phủ 3 trường hợp mảng/object thường gặp nhất của backend:
      const tempPassword = responseData?.createdUsers?.[0]?.temporaryPassword || "N/A";

      // 5. HIỂN THỊ MODAL BÁO THÀNH CÔNG (Block màn hình để copy)
      Modal.success({
        title: 'User created successfully',
        content: (
          <div style={{ marginTop: 16 }}>
            <p style={{ color: '#595959' }}>
              The user has been created. Please copy the temporary password below and send it to the user.
              <strong> It will not be shown again.</strong>
            </p>
            
            <div style={{ 
              marginTop: 16, 
              padding: '12px', 
              background: '#f5f5f5', 
              borderRadius: '8px',
              border: '1px solid #d9d9d9',
              textAlign: 'center'
            }}>
              <Paragraph 
                copyable={{ tooltips: ['Copy', 'Copied!'] }} 
                style={{ 
                  margin: 0, 
                  fontSize: '18px', 
                  fontWeight: 'bold', 
                  color: '#1677ff' 
                }}
              >
                {tempPassword}
              </Paragraph>
            </div>
          </div>
        ),
        okText: 'Close',
        onOk() {
          form.resetFields()
          if (onSuccess) onSuccess()
          onClose()
        }
      })
      
    } catch (error) {
      const errorData = error.response?.data
      
      if (errorData?.errors && typeof errorData.errors === 'object') {
        const errorMessages = Object.values(errorData.errors).flat()
        message.error(`Validation Error: ${errorMessages.join(', ')}`)
      } else {
        const errorMessage =
          errorData?.message ||
          errorData?.title ||
          'Unable to create user at this time.'
          
        message.error(errorMessage)
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <BaseDrawer
      title="Add users/groups"
      width={550}
      open={open}
      onClose={onClose}
      showFooter
      confirmLoading={loading}
      onConfirm={() => form.submit()}
    >
      <div
        style={{
          marginBottom: 24,
          padding: 12,
          background: '#fffbe6',
          border: '1px solid #ffe58f',
          borderRadius: 6,
        }}
      >
        Note: Local users cannot use Microsoft Teams related functions.
      </div>

      <Form form={form} layout="vertical" onFinish={handleSubmit}>
        <Form.Item label="Sign-in Method" name="signInMethod" initialValue="local">
          <Radio.Group>
            <Space direction="vertical">
              <Radio value="local">Local user</Radio>
              <Radio value="singpass">Singpass user</Radio>
            </Space>
          </Radio.Group>
        </Form.Item>

        <Form.Item label="Role" name="role" initialValue="tenant-user">
          <Radio.Group>
            <Space direction="vertical">
              <Radio value="tenant-user">Tenant user</Radio>
              <Radio value="tenant-admin">Tenant administrator</Radio>
            </Space>
          </Radio.Group>
        </Form.Item>

        <Form.Item
          label="Product"
          name="product"
          rules={[{ required: true, message: 'Please select a product' }]}
        >
          <TreeSelect
            treeData={productTreeData}
            treeCheckable
            showCheckedStrategy={TreeSelect.SHOW_PARENT}
            placeholder="Select products"
            style={{ width: '100%' }}
          />
        </Form.Item>

        <Form.Item label="User ID" name="userId" rules={[{ required: true, message: 'Please enter User ID' }]}>
          <Input />
        </Form.Item>

        <Form.Item label="Email Address" name="email" rules={[{ required: true, message: 'Please enter email' }]}>
          <Input />
        </Form.Item>

        <Form.Item label="Display Name" name="displayName" rules={[{ required: true, message: 'Please enter Display Name' }]}>
          <Input />
        </Form.Item>

        <Form.Item label="Staff & Student ID" name="staffId">
          <Input />
        </Form.Item>

        <Form.Item label="Sex" name="sex" initialValue="male">
          <Radio.Group>
            <Space>
              <Radio value="male">Male</Radio>
              <Radio value="female">Female</Radio>
            </Space>
          </Radio.Group>
        </Form.Item>

        <Form.Item
          label="Mobile Phone"
          name="mobilePhone"
          rules={[
            { pattern: /^[0-9+\-\s()]*$/, message: 'Please enter a valid phone number' },
          ]}
        >
          <Input placeholder="Enter mobile phone number" />
        </Form.Item>
      </Form>
    </BaseDrawer>
  )
}