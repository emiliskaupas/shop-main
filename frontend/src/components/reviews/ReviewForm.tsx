import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Rating,
  Box,
  Typography,
  Alert,
} from '@mui/material';
import { CreateReviewDto, UpdateReviewDto, ReviewDto } from '../../types/api';

interface ReviewFormProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (reviewData: CreateReviewDto | UpdateReviewDto) => Promise<void>;
  productId?: number;
  initialData?: ReviewDto;
  mode: 'create' | 'edit';
}

export const ReviewForm: React.FC<ReviewFormProps> = ({
  open,
  onClose,
  onSubmit,
  productId,
  initialData,
  mode,
}) => {
  const [rating, setRating] = useState<number>(initialData?.rating || 0);
  const [comment, setComment] = useState<string>(initialData?.comment || '');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async () => {
    if (rating === 0) {
      setError('Please provide a rating');
      return;
    }

    setIsSubmitting(true);
    setError(null);

    try {
      let reviewData: CreateReviewDto | UpdateReviewDto;
      
      if (mode === 'create') {
        if (!productId) {
          setError('Product ID is required');
          return;
        }
        reviewData = {
          productId,
          rating,
          comment: comment.trim() || undefined,
        };
      } else {
        reviewData = {
          rating,
          comment: comment.trim() || undefined,
        };
      }

      await onSubmit(reviewData);
      handleClose();
    } catch (err) {
      setError('Failed to save review. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    setRating(initialData?.rating || 0);
    setComment(initialData?.comment || '');
    setError(null);
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>
        {mode === 'create' ? 'Write a Review' : 'Edit Review'}
      </DialogTitle>
      
      <DialogContent>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}
        
        <Box sx={{ mb: 3 }}>
          <Typography component="legend" variant="subtitle1" gutterBottom>
            Rating *
          </Typography>
          <Rating
            name="rating"
            value={rating}
            onChange={(_, newValue) => {
              setRating(newValue || 0);
            }}
            size="large"
          />
        </Box>

        <TextField
          label="Comment (Optional)"
          multiline
          rows={4}
          fullWidth
          value={comment}
          onChange={(e) => setComment(e.target.value)}
          placeholder="Share your thoughts about this product..."
          variant="outlined"
          inputProps={{ maxLength: 1000 }}
          helperText={`${comment.length}/1000 characters`}
        />
      </DialogContent>
      
      <DialogActions>
        <Button onClick={handleClose} disabled={isSubmitting}>
          Cancel
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={isSubmitting || rating === 0}
        >
          {isSubmitting ? 'Saving...' : mode === 'create' ? 'Submit Review' : 'Update Review'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};