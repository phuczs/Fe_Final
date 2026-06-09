import api from './axios'

export const productsApi = {
  // GET /api/products
  getAllActive() {
    return api.get('/api/products')
  },

  // GET /api/products/favourites
  getFavourites() {
    return api.get('/api/products/favourites')
  },

  // POST /api/products/favourites/{id}/toggle
  toggleFavourite(productId) {
    return api.post(`/api/products/favourites/${productId}/toggle`)
  },
}
