// src/constants/products.js
// Single source of truth for all product metadata used across
// AccountPage, AssignProductsDrawer, and HomePage.

import {
  BookOutlined,
  IdcardOutlined,
  FileTextOutlined,
  TeamOutlined,
  SolutionOutlined,
} from '@ant-design/icons'

/** Map from integer product ID → human-readable label */
export const PRODUCT_LABEL_MAP = {
  1: 'Curricula · Student',
  2: 'Curricula · Staff',
  3: 'Vitae · Student',
  4: 'Vitae · Staff',
  5: 'Vitae · Trainer',
}

/** Short product name (without group prefix) */
export const PRODUCT_SHORT_NAME = {
  1: 'Student',
  2: 'Staff',
  3: 'Student',
  4: 'Staff',
  5: 'Trainer',
}

/** Group name per product */
export const PRODUCT_GROUP = {
  1: 'Curricula for Training',
  2: 'Curricula for Training',
  3: 'Vitae',
  4: 'Vitae',
  5: 'Vitae',
}

/** Ant Design tag color per product ID */
export const PRODUCT_TAG_COLORS = {
  1: 'blue',
  2: 'cyan',
  3: 'purple',
  4: 'geekblue',
  5: 'volcano',
}

/** Card accent color (CSS gradient) per product ID */
export const PRODUCT_CARD_GRADIENTS = {
  1: 'linear-gradient(135deg, #1d4ed8 0%, #3b82f6 100%)',
  2: 'linear-gradient(135deg, #0e7490 0%, #22d3ee 100%)',
  3: 'linear-gradient(135deg, #6d28d9 0%, #a78bfa 100%)',
  4: 'linear-gradient(135deg, #1e3a8a 0%, #60a5fa 100%)',
  5: 'linear-gradient(135deg, #c2410c 0%, #fb923c 100%)',
}

/** One-line description per product */
export const PRODUCT_DESCRIPTIONS = {
  1: 'Access training curricula designed for students.',
  2: 'Manage and deliver staff training programmes.',
  3: 'Build and share your student portfolio.',
  4: 'Create and maintain staff professional profiles.',
  5: 'Administer and track trainer certifications.',
}

/** Ant Design icon component per product ID */
export const PRODUCT_ICONS = {
  1: BookOutlined,
  2: IdcardOutlined,
  3: FileTextOutlined,
  4: TeamOutlined,
  5: SolutionOutlined,
}

/** TreeSelect data (used in AddUserDrawer and AssignProductsDrawer) */
export const PRODUCT_TREE_DATA = [
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

/** String-key → integer map (for form submission) */
export const PRODUCT_KEY_TO_ID = {
  'curricula-student': 1,
  'curricula-staff': 2,
  'vitae-student': 3,
  'vitae-staff': 4,
  'vitae-trainer': 5,
}

/** Integer → string-key map (for pre-filling TreeSelect) */
export const PRODUCT_ID_TO_KEY = Object.fromEntries(
  Object.entries(PRODUCT_KEY_TO_ID).map(([k, v]) => [v, k]),
)
