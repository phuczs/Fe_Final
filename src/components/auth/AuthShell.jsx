import {
  AppstoreOutlined,
  GlobalOutlined,
  TeamOutlined,
} from '@ant-design/icons'
import avepointLogo from '../../assets/avepoint-logo.png'
import './AuthShell.css'

const DEFAULT_HIGHLIGHTS = [
  {
    icon: AppstoreOutlined,
    title: 'Product navigation',
    description: 'Jump between SaaS products and managed workspaces in one place.',
  },
  {
    icon: TeamOutlined,
    title: 'User management',
    description: 'Organize accounts, permissions, and access across the platform.',
  },
  {
    icon: GlobalOutlined,
    title: 'Central entry point',
    description: 'A single login surface for AvePoint’s MOS platform and services.',
  },
]

const DEFAULT_STATS = [
  { value: '01', label: 'Unified portal' },
  { value: '24/7', label: 'Secure access' },
  { value: '100+', label: 'Connected workspaces' },
]

export default function AuthShell({
  eyebrow,
  title,
  description,
  layoutColumns = 'minmax(0, 1.05fr) minmax(360px, 0.95fr)',
  panelWidth = '460px',
  visualWidth = '88%',
  brandLabel = 'AvePoint MOS',
  brandTitle = 'Welcome to the management portal',
  brandDescription = 'AvePoint’s MOS platform is the core management page and login entry point for its SaaS products, supporting navigation to specific products, user management, and other functions.',
  highlights = DEFAULT_HIGHLIGHTS,
  stats = DEFAULT_STATS,
  children,
}) {
  return (
    <main
      className="auth-shell"
      style={{
        '--auth-shell-columns': layoutColumns,
        '--auth-panel-width': panelWidth,
        '--auth-visual-width': visualWidth,
      }}
    >
      <section className="auth-shell__visual" aria-hidden="true">
        <div className="auth-shell__orb auth-shell__orb--one" />
        <div className="auth-shell__orb auth-shell__orb--two" />
        <div className="auth-shell__brand-card">
          <div className="auth-shell__logo-wrap">
            <img className="auth-shell__logo" src={avepointLogo} alt="AvePoint" />
          </div>
          <span className="auth-shell__brand-label">{brandLabel}</span>
          <strong>{brandTitle}</strong>
          <p>{brandDescription}</p>
          <div className="auth-shell__stats">
            {stats.map((stat) => (
              <div key={stat.label} className="auth-shell__stat">
                <strong>{stat.value}</strong>
                <span>{stat.label}</span>
              </div>
            ))}
          </div>
          <div className="auth-shell__highlights">
            {highlights.map((highlight) => {
              const Icon = highlight.icon

              return (
                <div key={highlight.title} className="auth-shell__highlight">
                  <span className="auth-shell__highlight-icon" aria-hidden="true">
                    <Icon />
                  </span>
                  <div>
                    <h2>{highlight.title}</h2>
                    <p>{highlight.description}</p>
                  </div>
                </div>
              )
            })}
          </div>
        </div>
      </section>

      <section className="auth-shell__panel">
        <div className="auth-shell__panel-inner">
          <p className="auth-shell__eyebrow">{eyebrow}</p>
          <h1>{title}</h1>
          <p className="auth-shell__description">{description}</p>
          {children}
        </div>
      </section>
    </main>
  )
}
