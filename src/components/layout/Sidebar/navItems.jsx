import {
  HomeOutlined,
  TeamOutlined,
  KeyOutlined,
  SettingOutlined,
  BarChartOutlined,
  MailOutlined,
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
    label: 'Email Whitelist',
    path: '/settings/email-whitelist',
    icon: <MailOutlined />,
  },
  {
    label: 'Reports',
    path: '/reports/user-activity',
    icon: <BarChartOutlined />,
  },
]