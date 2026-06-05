import { Outlet } from 'react-router-dom'

import Sidebar from '../components/layout/Sidebar/Sidebar'
import Header from '../components/layout/Header/Header'
import Breadcrumb from '../components/layout/Breadcrumb/Breadcrumb'

import './HomeLayout.css'

export default function HomeLayout() {
  return (
    <div className="layout">
      <Sidebar />

      <div className="main">
        <Header />

        <Breadcrumb />

        <main className="content">
          <Outlet />
        </main>
      </div>
    </div>
  )
}