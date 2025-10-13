import React, { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  Divider,
  Alert,
  CircularProgress,
  Rating,
  Paper,
} from '@mui/material';
import { Add } from '@mui/icons-material';
import { ReviewDto, CreateReviewDto, UpdateReviewDto } from '../../types/api';
import { ReviewItem } from './ReviewItem';
import { ReviewForm } from './ReviewForm';

interface ReviewListProps {
  productId: number;
  reviews: ReviewDto[];
  averageRating?: number;
  reviewCount?: number;
  isLoading?: boolean;
  error?: string | null;
  canCreateReview?: boolean;
  currentUserId?: number;
  onCreateReview: (reviewData: CreateReviewDto) => Promise<void>;
  onUpdateReview: (reviewId: number, reviewData: UpdateReviewDto) => Promise<void>;
  onDeleteReview: (reviewId: number) => Promise<void>;
  onRefresh?: () => void;
}

export const ReviewList: React.FC<ReviewListProps> = ({
  productId,
  reviews,
  averageRating,
  reviewCount,
  isLoading = false,
  error = null,
  canCreateReview = false,
  currentUserId,
  onCreateReview,
  onUpdateReview,
  onDeleteReview,
  onRefresh,
}) => {
  const [showForm, setShowForm] = useState(false);
  const [editingReview, setEditingReview] = useState<ReviewDto | null>(null);

  const handleCreateReview = async (reviewData: CreateReviewDto | UpdateReviewDto) => {
    await onCreateReview(reviewData as CreateReviewDto);
    setShowForm(false);
    if (onRefresh) {
      onRefresh();
    }
  };

  const handleUpdateReview = async (reviewData: CreateReviewDto | UpdateReviewDto) => {
    if (editingReview) {
      await onUpdateReview(editingReview.id, reviewData as UpdateReviewDto);
      setEditingReview(null);
      if (onRefresh) {
        onRefresh();
      }
    }
  };

  const handleDeleteReview = async (reviewId: number) => {
    await onDeleteReview(reviewId);
    if (onRefresh) {
      onRefresh();
    }
  };

  const handleEditReview = (review: ReviewDto) => {
    setEditingReview(review);
  };

  const userHasReviewed = currentUserId
    ? reviews.some(review => review.userId === currentUserId)
    : false;

  return (
    <Box>
      {/* Header with overall rating */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          Customer Reviews
        </Typography>
        
        {reviewCount !== undefined && reviewCount > 0 ? (
          <Box display="flex" alignItems="center" gap={2}>
            <Rating
              value={averageRating || 0}
              precision={0.1}
              readOnly
              size="large"
            />
            <Typography variant="h6">
              {averageRating?.toFixed(1) || '0.0'} out of 5
            </Typography>
            <Typography variant="body2" color="text.secondary">
              ({reviewCount} {reviewCount === 1 ? 'review' : 'reviews'})
            </Typography>
          </Box>
        ) : (
          <Typography variant="body2" color="text.secondary">
            No reviews yet. Be the first to review this product!
          </Typography>
        )}

        {canCreateReview && !userHasReviewed && (
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => setShowForm(true)}
            sx={{ mt: 2 }}
          >
            Write a Review
          </Button>
        )}
        
        {userHasReviewed && (
          <Alert severity="info" sx={{ mt: 2 }}>
            You have already reviewed this product. You can edit or delete your review below.
          </Alert>
        )}
      </Paper>

      {/* Error state */}
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      {/* Loading state */}
      {isLoading && (
        <Box display="flex" justifyContent="center" p={3}>
          <CircularProgress />
        </Box>
      )}

      {/* Reviews list */}
      {!isLoading && reviews.length > 0 && (
        <Box>
          <Divider sx={{ mb: 2 }} />
          {reviews.map((review) => (
            <ReviewItem
              key={review.id}
              review={review}
              canEdit={currentUserId === review.userId}
              canDelete={currentUserId === review.userId}
              onEdit={handleEditReview}
              onDelete={handleDeleteReview}
            />
          ))}
        </Box>
      )}

      {/* Empty state */}
      {!isLoading && reviews.length === 0 && !error && (
        <Box textAlign="center" py={4}>
          <Typography variant="body1" color="text.secondary">
            No reviews available for this product.
          </Typography>
        </Box>
      )}

      {/* Create review form */}
      <ReviewForm
        open={showForm}
        onClose={() => setShowForm(false)}
        onSubmit={handleCreateReview}
        productId={productId}
        mode="create"
      />

      {/* Edit review form */}
      <ReviewForm
        open={!!editingReview}
        onClose={() => setEditingReview(null)}
        onSubmit={handleUpdateReview}
        initialData={editingReview || undefined}
        mode="edit"
      />
    </Box>
  );
};