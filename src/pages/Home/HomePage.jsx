import { useCallback, useEffect, useState } from 'react'
import { Tooltip, message } from 'antd'
import { AppstoreOutlined, StarFilled, StarOutlined } from '@ant-design/icons'

import { usersApi } from '../../api/usersApi'
import { productsApi } from '../../api/productsApi'
import {
  PRODUCT_CARD_GRADIENTS,
  PRODUCT_DESCRIPTIONS,
  PRODUCT_GROUP,
  PRODUCT_ICONS,
  PRODUCT_LABEL_MAP,
  PRODUCT_SHORT_NAME,
} from '../../constants/products'

import './HomePage.css'

// ── ProductCard ───────────────────────────────────────────────────────────────

function ProductCard({ productId, isFavourite, onToggleFavourite }) {
  const Icon = PRODUCT_ICONS[productId]
  const gradient = PRODUCT_CARD_GRADIENTS[productId] || '#667eea'
  const group = PRODUCT_GROUP[productId] || ''
  const name = PRODUCT_SHORT_NAME[productId] || PRODUCT_LABEL_MAP[productId]
  const desc = PRODUCT_DESCRIPTIONS[productId] || ''

  return (
    <div className="home-product-card">
      {/* top colour bar */}
      <div
        className="home-product-card__bar"
        style={{ background: gradient }}
      />

      {/* star toggle */}
      <Tooltip title={isFavourite ? 'Remove from favourites' : 'Add to favourites'}>
        <button
          className={`home-product-card__star${isFavourite ? ' home-product-card__star--active' : ''}`}
          onClick={(e) => {
            e.stopPropagation()
            onToggleFavourite(productId)
          }}
          aria-label={isFavourite ? 'Remove from favourites' : 'Add to favourites'}
        >
          {isFavourite ? <StarFilled /> : <StarOutlined />}
        </button>
      </Tooltip>

      <div className="home-product-card__body">
        {/* icon */}
        <div
          className="home-product-card__icon-wrap"
          style={{ background: gradient }}
        >
          {Icon && <Icon />}
        </div>

        <p className="home-product-card__group">{group}</p>
        <h3 className="home-product-card__name">{name}</h3>
        <p className="home-product-card__desc">{desc}</p>
      </div>
    </div>
  )
}

// ── SkeletonGrid ──────────────────────────────────────────────────────────────

function SkeletonGrid({ count = 4 }) {
  return (
    <div className="home-skeleton-grid">
      {Array.from({ length: count }).map((_, i) => (
        <div key={i} className="home-skeleton-card" />
      ))}
    </div>
  )
}

// ── HomePage ──────────────────────────────────────────────────────────────────

export default function HomePage() {
  const [profile, setProfile] = useState(null)
  const [loading, setLoading] = useState(true)
  const [favourites, setFavourites] = useState([])

  // Fetch profile and favourites concurrently
  useEffect(() => {
    const fetch = async () => {
      setLoading(true)
      try {
        const [profileRes, favsRes] = await Promise.all([
          usersApi.getMyProfile(),
          productsApi.getFavourites()
        ])
        
        setProfile(profileRes.data)
        setFavourites(favsRes.data.map(f => f.productId))
      } catch {
        message.error('Failed to load your services. Please refresh.')
      } finally {
        setLoading(false)
      }
    }
    fetch()
  }, [])

  const toggleFavourite = useCallback(async (productId) => {
    // Optimistic UI update
    const isAdding = !favourites.includes(productId)
    
    setFavourites((prev) => {
      if (isAdding) {
        return [...prev, productId]
      }
      return prev.filter(id => id !== productId)
    })

    try {
      await productsApi.toggleFavourite(productId)
      message.open({
        type: 'success',
        content: isAdding
          ? `Added "${PRODUCT_LABEL_MAP[productId]}" to favourites`
          : `Removed "${PRODUCT_LABEL_MAP[productId]}" from favourites`,
        duration: 2,
      })
    } catch {
      // Revert if API fails
      setFavourites((prev) => {
        if (isAdding) {
          return prev.filter(id => id !== productId)
        }
        return [...prev, productId]
      })
      message.error('Failed to update favourite. Please try again.')
    }
  }, [favourites])

  // Assigned product IDs (only those in the master catalogue)
  const assignedIds = (profile?.productIds ?? []).filter(
    (id) => PRODUCT_LABEL_MAP[id] !== undefined,
  )

  // Partition: favourited vs rest
  const favouritedIds = assignedIds.filter((id) => favourites.includes(id))
  const restIds = assignedIds.filter((id) => !favourites.includes(id))

  const displayName = profile?.displayName?.split(' ')[0] || 'there'

  return (
    <div className="home-page">
      {/* ── welcome banner ────────────────────────────────────────────── */}
      <div className="home-banner">
        <div className="home-banner__text">
          <h1>Welcome back, {displayName} 👋</h1>
          <p>
            {loading
              ? 'Loading your assigned services…'
              : assignedIds.length > 0
              ? `You have access to ${assignedIds.length} service${assignedIds.length !== 1 ? 's' : ''}. Star the ones you use most!`
              : 'No services have been assigned to your account yet.'}
          </p>
        </div>
        <div className="home-banner__emoji" aria-hidden="true">🚀</div>
      </div>

      {loading ? (
        /* ── loading state ──────────────────────────────────────────── */
        <section>
          <div className="home-section__header">
            <span className="home-section__icon"><AppstoreOutlined /></span>
            <h2 className="home-section__title">Your Services</h2>
          </div>
          <SkeletonGrid count={4} />
        </section>
      ) : assignedIds.length === 0 ? (
        /* ── empty state ────────────────────────────────────────────── */
        <div className="home-empty">
          <div className="home-empty__icon">📭</div>
          <h2 className="home-empty__title">No services assigned</h2>
          <p className="home-empty__sub">
            Contact your administrator to have products assigned to your account.
          </p>
        </div>
      ) : (
        <>
          {/* ── favourite services ────────────────────────────────────── */}
          {favouritedIds.length > 0 && (
            <section>
              <div className="home-section__header">
                <span className="home-section__icon">⭐</span>
                <h2 className="home-section__title">Favourite Services</h2>
                <span className="home-section__count">{favouritedIds.length}</span>
              </div>
              <div className="home-products-grid">
                {favouritedIds.map((id) => (
                  <ProductCard
                    key={id}
                    productId={id}
                    isFavourite
                    onToggleFavourite={toggleFavourite}
                  />
                ))}
              </div>
            </section>
          )}

          {/* ── all assigned services ─────────────────────────────────── */}
          <section>
            <div className="home-section__header">
              <span className="home-section__icon">
                <AppstoreOutlined />
              </span>
              <h2 className="home-section__title">Your Services</h2>
              <span className="home-section__count">{assignedIds.length}</span>
            </div>

            <div className="home-products-grid">
              {restIds.map((id) => (
                <ProductCard
                  key={id}
                  productId={id}
                  isFavourite={false}
                  onToggleFavourite={toggleFavourite}
                />
              ))}

              {/* If everything is favourited, show all under "Your Services" too */}
              {restIds.length === 0 &&
                favouritedIds.map((id) => (
                  <ProductCard
                    key={id}
                    productId={id}
                    isFavourite
                    onToggleFavourite={toggleFavourite}
                  />
                ))}
            </div>
          </section>
        </>
      )}
    </div>
  )
}