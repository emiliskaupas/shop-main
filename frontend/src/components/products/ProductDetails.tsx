import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Container,
  Grid,
  Typography,
  Button,
  Card,
  CardContent,
  CardMedia,
  Chip,
  Avatar,
  CircularProgress,
  Alert,
  Rating,
  Divider,
} from '@mui/material';
import {
  ArrowBack as ArrowBackIcon,
  ShoppingCart as CartIcon,
  Edit as EditIcon,
} from '@mui/icons-material';
import { useProductsStore } from '../../stores/productsStore';
import { useReviewsStore } from '../../stores/reviewsStore';
import { useAuthStore } from '../../stores/authStore';
import { useCartStore } from '../../stores/cartStore';
import { ReviewList } from '../reviews';
import { ProductDto, CreateReviewDto, UpdateReviewDto } from '../../types/api';
import { ProductType } from '../../types/enums';

const getProductTypeLabel = (type: ProductType): string => {
  return ProductType[type];
};

export const ProductDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  
  // State
  const [product, setProduct] = useState<ProductDto | null>(null);
  const [isLoadingProduct, setIsLoadingProduct] = useState(true);
  const [isAddingToCart, setIsAddingToCart] = useState(false);
  
  // Stores
  const { getProductById } = useProductsStore();
  const { 
    productReviews, 
    productRatings, 
    productReviewCounts,
    isLoading: isLoadingReviews,
    error: reviewsError,
    loadReviewsByProduct,
    createReview,
    updateReview,
    deleteReview,
    loadProductRating,
    loadProductReviewCount,
  } = useReviewsStore();
  const { user } = useAuthStore();
  const { addToCart } = useCartStore();

  const productId = parseInt(id || '0');
  const reviews = productReviews[productId] || [];
  const averageRating = productRatings[productId];
  const reviewCount = productReviewCounts[productId];

  // Load product and reviews data
  useEffect(() => {
    const loadProduct = async () => {
      if (!productId || productId <= 0) {
        navigate('/products');
        return;
      }

      setIsLoadingProduct(true);
      try {
        const productData = await getProductById(productId);
        if (productData) {
          setProduct(productData);
        } else {
          navigate('/products');
        }
      } catch (error) {
        console.error('Failed to load product:', error);
        navigate('/products');
      } finally {
        setIsLoadingProduct(false);
      }
    };

    loadProduct();
  }, [productId, getProductById, navigate]);

  useEffect(() => {
    if (productId > 0) {
      loadReviewsByProduct(productId);
      loadProductRating(productId);
      loadProductReviewCount(productId);
    }
  }, [productId, loadReviewsByProduct, loadProductRating, loadProductReviewCount]);

  const handleAddToCart = async () => {
    if (!product || !user) return;

    setIsAddingToCart(true);
    try {
      await addToCart(product.id, 1, product);
    } catch (error) {
      console.error('Failed to add to cart:', error);
    } finally {
      setIsAddingToCart(false);
    }
  };

  const handleCreateReview = async (reviewData: CreateReviewDto) => {
    await createReview(reviewData);
  };

  const handleUpdateReview = async (reviewId: number, reviewData: UpdateReviewDto) => {
    await updateReview(reviewId, reviewData);
  };

  const handleDeleteReview = async (reviewId: number) => {
    await deleteReview(reviewId);
  };

  const handleRefreshReviews = () => {
    loadReviewsByProduct(productId);
    loadProductRating(productId);
    loadProductReviewCount(productId);
  };

  const handleBack = () => {
    navigate('/products');
  };

  const handleEditProduct = () => {
    navigate(`/products/${productId}/edit`);
  };

  if (isLoadingProduct) {
    return (
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
          <CircularProgress size={60} />
        </Box>
      </Container>
    );
  }

  if (!product) {
    return (
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <Alert severity="error">Product not found</Alert>
      </Container>
    );
  }

  const isOwner = user && user.id === product.createdByUserId;
  const canReview = user && !isOwner;

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      {/* Back Button */}
      <Button
        startIcon={<ArrowBackIcon />}
        onClick={handleBack}
        sx={{ mb: 3 }}
      >
        Back to Products
      </Button>

      <Grid container spacing={4}>
        {/* Product Information */}
        <Grid size={{ xs: 12, md: 6 }}>
          <Card>
            <CardMedia sx={{ height: 400 }}>
              {product.imageUrl ? (
                <img
                  src={product.imageUrl}
                  alt={product.name}
                  style={{
                    width: '100%',
                    height: '100%',
                    objectFit: 'cover',
                  }}
                />
              ) : (
                <Box
                  sx={{
                    width: '100%',
                    height: '100%',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    bgcolor: 'grey.200',
                  }}
                >
                  <Avatar sx={{ width: 120, height: 120 }}>
                    {product.name.charAt(0)}
                  </Avatar>
                </Box>
              )}
            </CardMedia>
          </Card>
        </Grid>

        <Grid size={{ xs: 12, md: 6 }}>
          <Box>
            <Typography variant="h4" gutterBottom>
              {product.name}
            </Typography>

            <Box display="flex" alignItems="center" gap={2} mb={2}>
              <Chip 
                label={getProductTypeLabel(product.productType)} 
                color="primary"
                variant="outlined"
              />
              <Typography variant="h5" color="primary">
                ${product.price.toFixed(2)}
              </Typography>
            </Box>

            {/* Rating Display */}
            {averageRating !== undefined && reviewCount !== undefined && reviewCount > 0 && (
              <Box display="flex" alignItems="center" mb={2}>
                <Rating 
                  value={averageRating} 
                  precision={0.1} 
                  readOnly 
                  size="large"
                />
                <Typography variant="h6" sx={{ ml: 1 }}>
                  {averageRating.toFixed(1)}
                </Typography>
                <Typography variant="body2" color="text.secondary" sx={{ ml: 1 }}>
                  ({reviewCount} {reviewCount === 1 ? 'review' : 'reviews'})
                </Typography>
              </Box>
            )}

            <Typography variant="body1" paragraph>
              {product.shortDescription}
            </Typography>

            <Box mb={3}>
              <Typography variant="subtitle2" color="text.secondary">
                Created by: {product.createdByUserName}
              </Typography>
              <Typography variant="subtitle2" color="text.secondary">
                Email: {product.createdByEmail}
              </Typography>
              <Typography variant="subtitle2" color="text.secondary">
                Created: {new Date(product.createdAt).toLocaleDateString()}
              </Typography>
            </Box>

            {/* Action Buttons */}
            <Box display="flex" gap={2}>
              {isOwner ? (
                <Button
                  variant="contained"
                  startIcon={<EditIcon />}
                  onClick={handleEditProduct}
                  fullWidth
                >
                  Edit Product
                </Button>
              ) : (
                <Button
                  variant="contained"
                  startIcon={isAddingToCart ? <CircularProgress size={16} /> : <CartIcon />}
                  onClick={handleAddToCart}
                  disabled={isAddingToCart || !user}
                  fullWidth
                >
                  {isAddingToCart ? 'Adding...' : 'Add to Cart'}
                </Button>
              )}
            </Box>

            {!user && (
              <Alert severity="info" sx={{ mt: 2 }}>
                Please log in to add items to cart or write reviews.
              </Alert>
            )}
          </Box>
        </Grid>
      </Grid>

      <Divider sx={{ my: 4 }} />

      {/* Reviews Section */}
      <ReviewList
        productId={productId}
        reviews={reviews}
        averageRating={averageRating}
        reviewCount={reviewCount}
        isLoading={isLoadingReviews}
        error={reviewsError}
        canCreateReview={!!canReview}
        currentUserId={user?.id}
        onCreateReview={handleCreateReview}
        onUpdateReview={handleUpdateReview}
        onDeleteReview={handleDeleteReview}
        onRefresh={handleRefreshReviews}
      />
    </Container>
  );
};