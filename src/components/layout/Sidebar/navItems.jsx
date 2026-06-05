import {
  HomeOutlined,
  TeamOutlined,
  KeyOutlined,
  SettingOutlined,
  BarChartOutlined,
} from '@ant-design/icons'

export const navItems = [
  {
    label: 'Home',
    path: '/home',
    icon: <HomeOutlined />,
  },
  {
    label: 'Account',
    path: '/account',
    icon: <TeamOutlined />,
  },
  {
    label: 'Subscription',
    path: '/subscription',
    icon: <KeyOutlined />,
  },
  {
    label: 'Settings',
    path: '/settings/two-factor-authentication',
    icon: <SettingOutlined />,
  },
  {
    label: 'Reports',
    path: '/reports/user-activity',
    icon: <BarChartOutlined />,
  },
]