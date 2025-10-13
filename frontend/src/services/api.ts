import axios, { AxiosResponse } from 'axios';
import {
  LoginDto,
  RegisterDto,
  LoginResponseDto,
  UserDto,
  ProductDto,
  CreateProductDto,
  UpdateProductDto,
  CartItemDto,
  AddToCartRequestDto,
  UpdateQuantityRequestDto,
  PaginationRequest,
  PagedResult,
  NotificationStatus,
  ApiResponse,
  ApiErrorResponse,
  ReviewDto,
  CreateReviewDto,
  UpdateReviewDto,
} from '../types/api';

// Create axios instance with base configuration
const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor for consistent error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Handle common error scenarios
    if (error.response?.status === 401) {
      // Token expired or invalid
      localStorage.removeItem('authToken');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Helper function to handle API responses
const handleResponse = <T>(response: AxiosResponse<T>): T => {
  return response.data;
};

const handleError = (error: any): ApiErrorResponse => {
  if (error.response?.data?.error) {
    return { error: error.response.data.error, success: false };
  }
  return { error: 'An unexpected error occurred', success: false };
};

// Authentication API
export const authApi = {
  login: async (loginDto: LoginDto): Promise<ApiResponse<LoginResponseDto>> => {
    try {
      const response = await api.post<LoginResponseDto>('/auth/login', loginDto);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  register: async (registerDto: RegisterDto): Promise<ApiResponse<UserDto>> => {
    try {
      const response = await api.post<UserDto>('/auth/register', registerDto);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },
};

// Products API
export const productsApi = {
  getProducts: async (params: PaginationRequest): Promise<ApiResponse<PagedResult<ProductDto>>> => {
    try {
      const response = await api.get<PagedResult<ProductDto>>('/products', { params });
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  getProductById: async (id: number): Promise<ApiResponse<ProductDto>> => {
    try {
      const response = await api.get<ProductDto>(`/products/${id}`);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  getMyProducts: async (params: PaginationRequest): Promise<ApiResponse<PagedResult<ProductDto>>> => {
    try {
      const response = await api.get<PagedResult<ProductDto>>('/products/my-products', { params });
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  createProduct: async (productDto: CreateProductDto): Promise<ApiResponse<ProductDto>> => {
    try {
      const response = await api.post<ProductDto>('/products', productDto);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  updateProduct: async (id: number, productDto: UpdateProductDto): Promise<ApiResponse<ProductDto>> => {
    try {
      const response = await api.put<ProductDto>(`/products/${id}`, productDto);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  deleteProduct: async (id: number): Promise<ApiResponse<void>> => {
    try {
      await api.delete(`/products/${id}`);
      return { data: undefined, success: true };
    } catch (error) {
      return handleError(error);
    }
  },
};

// Cart API
export const cartApi = {
  getCartItems: async (userId: number): Promise<ApiResponse<CartItemDto[]>> => {
    try {
      const response = await api.get<CartItemDto[]>(`/cart/${userId}`);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  addToCart: async (userId: number, request: AddToCartRequestDto): Promise<ApiResponse<CartItemDto>> => {
    try {
      const response = await api.post<CartItemDto>(`/cart/${userId}/items`, request);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  updateCartItemQuantity: async (
    userId: number,
    cartItemId: number,
    request: UpdateQuantityRequestDto
  ): Promise<ApiResponse<CartItemDto>> => {
    try {
      const response = await api.put<CartItemDto>(`/cart/${userId}/items/${cartItemId}`, request);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  removeFromCart: async (userId: number, cartItemId: number): Promise<ApiResponse<void>> => {
    try {
      await api.delete(`/cart/${userId}/items/${cartItemId}`);
      return { data: undefined, success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  clearCart: async (userId: number): Promise<ApiResponse<void>> => {
    try {
      await api.delete(`/cart/${userId}/clear`);
      return { data: undefined, success: true };
    } catch (error) {
      return handleError(error);
    }
  },
};

// Reviews API
export const reviewsApi = {
  getReviewsByProduct: async (productId: number): Promise<ApiResponse<ReviewDto[]>> => {
    try {
      const response = await api.get<ReviewDto[]>(`/reviews/product/${productId}`);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  getReviewsByUser: async (userId: number): Promise<ApiResponse<ReviewDto[]>> => {
    try {
      const response = await api.get<ReviewDto[]>(`/reviews/user/${userId}`);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  getMyReviews: async (): Promise<ApiResponse<ReviewDto[]>> => {
    try {
      const response = await api.get<ReviewDto[]>('/reviews/my-reviews');
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  getReviewById: async (id: number): Promise<ApiResponse<ReviewDto>> => {
    try {
      const response = await api.get<ReviewDto>(`/reviews/${id}`);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  createReview: async (reviewDto: CreateReviewDto): Promise<ApiResponse<ReviewDto>> => {
    try {
      const response = await api.post<ReviewDto>('/reviews', reviewDto);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  updateReview: async (id: number, reviewDto: UpdateReviewDto): Promise<ApiResponse<ReviewDto>> => {
    try {
      const response = await api.put<ReviewDto>(`/reviews/${id}`, reviewDto);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  deleteReview: async (id: number): Promise<ApiResponse<void>> => {
    try {
      await api.delete(`/reviews/${id}`);
      return { data: undefined, success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  getProductAverageRating: async (productId: number): Promise<ApiResponse<number>> => {
    try {
      const response = await api.get<number>(`/reviews/product/${productId}/average-rating`);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  getProductReviewCount: async (productId: number): Promise<ApiResponse<number>> => {
    try {
      const response = await api.get<number>(`/reviews/product/${productId}/count`);
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },
};

// Notifications API
export const notificationsApi = {
  getStatus: async (): Promise<ApiResponse<NotificationStatus>> => {
    try {
      const response = await api.get<NotificationStatus>('/notification/status');
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  getLatest: async (): Promise<ApiResponse<any>> => {
    try {
      const response = await api.get('/notification/latest');
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },

  getAll: async (): Promise<ApiResponse<any[]>> => {
    try {
      const response = await api.get<any[]>('/notification/all');
      return { data: handleResponse(response), success: true };
    } catch (error) {
      return handleError(error);
    }
  },
};

export default api;