import { create } from 'zustand';
import { ReviewDto, CreateReviewDto, UpdateReviewDto } from '../types/api';
import { reviewsApi } from '../services/api';

interface ReviewsState {
  reviews: ReviewDto[];
  userReviews: ReviewDto[];
  productReviews: { [productId: number]: ReviewDto[] };
  productRatings: { [productId: number]: number };
  productReviewCounts: { [productId: number]: number };
  isLoading: boolean;
  error: string | null;
}

interface ReviewsActions {
  loadReviewsByProduct: (productId: number) => Promise<void>;
  loadReviewsByUser: (userId: number) => Promise<void>;
  loadMyReviews: () => Promise<void>;
  getReviewById: (id: number) => Promise<ReviewDto | null>;
  createReview: (reviewDto: CreateReviewDto) => Promise<ReviewDto | null>;
  updateReview: (id: number, reviewDto: UpdateReviewDto) => Promise<ReviewDto | null>;
  deleteReview: (id: number) => Promise<boolean>;
  loadProductRating: (productId: number) => Promise<number | null>;
  loadProductReviewCount: (productId: number) => Promise<number | null>;
  clearError: () => void;
  resetReviews: () => void;
}

export const useReviewsStore = create<ReviewsState & ReviewsActions>((set, get) => ({
  // State
  reviews: [],
  userReviews: [],
  productReviews: {},
  productRatings: {},
  productReviewCounts: {},
  isLoading: false,
  error: null,

  // Actions
  loadReviewsByProduct: async (productId: number) => {
    set({ isLoading: true, error: null });
    
    try {
      const response = await reviewsApi.getReviewsByProduct(productId);
      
      if (response.success && 'data' in response) {
        const reviews = response.data;
        set(state => ({
          productReviews: {
            ...state.productReviews,
            [productId]: reviews,
          },
        }));
      } else {
        set({ error: 'error' in response ? response.error : 'Failed to load reviews' });
      }
    } catch (error) {
      set({ error: 'Failed to load reviews' });
    } finally {
      set({ isLoading: false });
    }
  },

  loadReviewsByUser: async (userId: number) => {
    set({ isLoading: true, error: null });
    
    try {
      const response = await reviewsApi.getReviewsByUser(userId);
      
      if (response.success && 'data' in response) {
        set({ userReviews: response.data });
      } else {
        set({ error: 'error' in response ? response.error : 'Failed to load user reviews' });
      }
    } catch (error) {
      set({ error: 'Failed to load user reviews' });
    } finally {
      set({ isLoading: false });
    }
  },

  loadMyReviews: async () => {
    set({ isLoading: true, error: null });
    
    try {
      const response = await reviewsApi.getMyReviews();
      
      if (response.success && 'data' in response) {
        set({ userReviews: response.data });
      } else {
        set({ error: 'error' in response ? response.error : 'Failed to load my reviews' });
      }
    } catch (error) {
      set({ error: 'Failed to load my reviews' });
    } finally {
      set({ isLoading: false });
    }
  },

  getReviewById: async (id: number) => {
    set({ isLoading: true, error: null });
    
    try {
      const response = await reviewsApi.getReviewById(id);
      
      if (response.success && 'data' in response) {
        return response.data;
      } else {
        set({ error: 'error' in response ? response.error : 'Failed to load review' });
        return null;
      }
    } catch (error) {
      set({ error: 'Failed to load review' });
      return null;
    } finally {
      set({ isLoading: false });
    }
  },

  createReview: async (reviewDto: CreateReviewDto) => {
    set({ isLoading: true, error: null });
    
    try {
      const response = await reviewsApi.createReview(reviewDto);
      
      if (response.success && 'data' in response) {
        const newReview = response.data;
        
        // Update product reviews
        set(state => ({
          productReviews: {
            ...state.productReviews,
            [reviewDto.productId]: [
              newReview,
              ...(state.productReviews[reviewDto.productId] || []),
            ],
          },
        }));
        
        // Refresh product rating and count
        get().loadProductRating(reviewDto.productId);
        get().loadProductReviewCount(reviewDto.productId);
        
        return newReview;
      } else {
        set({ error: 'error' in response ? response.error : 'Failed to create review' });
        return null;
      }
    } catch (error) {
      set({ error: 'Failed to create review' });
      return null;
    } finally {
      set({ isLoading: false });
    }
  },

  updateReview: async (id: number, reviewDto: UpdateReviewDto) => {
    set({ isLoading: true, error: null });
    
    try {
      const response = await reviewsApi.updateReview(id, reviewDto);
      
      if (response.success && 'data' in response) {
        const updatedReview = response.data;
        
        // Update product reviews
        set(state => {
          const newProductReviews = { ...state.productReviews };
          
          // Find and update the review in the appropriate product's reviews
          Object.keys(newProductReviews).forEach(productIdStr => {
            const productId = parseInt(productIdStr);
            const reviews = newProductReviews[productId];
            const reviewIndex = reviews.findIndex(r => r.id === id);
            
            if (reviewIndex !== -1) {
              newProductReviews[productId] = [
                ...reviews.slice(0, reviewIndex),
                updatedReview,
                ...reviews.slice(reviewIndex + 1),
              ];
              
              // Refresh product rating
              get().loadProductRating(productId);
            }
          });
          
          return { productReviews: newProductReviews };
        });
        
        return updatedReview;
      } else {
        set({ error: 'error' in response ? response.error : 'Failed to update review' });
        return null;
      }
    } catch (error) {
      set({ error: 'Failed to update review' });
      return null;
    } finally {
      set({ isLoading: false });
    }
  },

  deleteReview: async (id: number) => {
    set({ isLoading: true, error: null });
    
    try {
      const response = await reviewsApi.deleteReview(id);
      
      if (response.success) {
        // Remove review from product reviews
        set(state => {
          const newProductReviews = { ...state.productReviews };
          let productIdToRefresh: number | null = null;
          
          Object.keys(newProductReviews).forEach(productIdStr => {
            const productId = parseInt(productIdStr);
            const reviews = newProductReviews[productId];
            const reviewIndex = reviews.findIndex(r => r.id === id);
            
            if (reviewIndex !== -1) {
              newProductReviews[productId] = reviews.filter(r => r.id !== id);
              productIdToRefresh = productId;
            }
          });
          
          // Refresh product rating and count
          if (productIdToRefresh) {
            get().loadProductRating(productIdToRefresh);
            get().loadProductReviewCount(productIdToRefresh);
          }
          
          return { productReviews: newProductReviews };
        });
        
        return true;
      } else {
        set({ error: 'error' in response ? response.error : 'Failed to delete review' });
        return false;
      }
    } catch (error) {
      set({ error: 'Failed to delete review' });
      return false;
    } finally {
      set({ isLoading: false });
    }
  },

  loadProductRating: async (productId: number) => {
    try {
      const response = await reviewsApi.getProductAverageRating(productId);
      
      if (response.success && 'data' in response) {
        set(state => ({
          productRatings: {
            ...state.productRatings,
            [productId]: response.data,
          },
        }));
        return response.data;
      }
      return null;
    } catch (error) {
      return null;
    }
  },

  loadProductReviewCount: async (productId: number) => {
    try {
      const response = await reviewsApi.getProductReviewCount(productId);
      
      if (response.success && 'data' in response) {
        set(state => ({
          productReviewCounts: {
            ...state.productReviewCounts,
            [productId]: response.data,
          },
        }));
        return response.data;
      }
      return null;
    } catch (error) {
      return null;
    }
  },

  clearError: () => {
    set({ error: null });
  },

  resetReviews: () => {
    set({
      reviews: [],
      userReviews: [],
      productReviews: {},
      productRatings: {},
      productReviewCounts: {},
      isLoading: false,
      error: null,
    });
  },
}));