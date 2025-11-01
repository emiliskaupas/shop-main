import React from 'react';
import {
  Card,
  CardContent,
  Typography,
  Rating,
  Box,
  Avatar,
  IconButton,
  Menu,
  MenuItem,
} from '@mui/material';
import { MoreVert } from '@mui/icons-material';
import { ReviewDto } from '../../types/api';

interface ReviewItemProps {
  review: ReviewDto;
  canEdit?: boolean;
  canDelete?: boolean;
  onEdit?: (review: ReviewDto) => void;
  onDelete?: (reviewId: number) => void;
}

export const ReviewItem: React.FC<ReviewItemProps> = ({
  review,
  canEdit = false,
  canDelete = false,
  onEdit,
  onDelete,
}) => {
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleEdit = () => {
    if (onEdit) {
      onEdit(review);
    }
    handleClose();
  };

  const handleDelete = () => {
    if (onDelete) {
      onDelete(review.id);
    }
    handleClose();
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  return (
    <Card sx={{ mb: 2 }}>
      <CardContent>
        <Box display="flex" justifyContent="space-between" alignItems="flex-start">
          <Box display="flex" alignItems="center" mb={1}>
            <Avatar sx={{ mr: 2, bgcolor: 'primary.main' }}>
              {getInitials(review.username || 'Anonymous')}
            </Avatar>
            <Box>
              <Typography variant="subtitle2" component="div">
                {review.username || 'Anonymous'}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {formatDate(review.createdAt)}
              </Typography>
            </Box>
          </Box>
          
          {(canEdit || canDelete) && (
            <>
              <IconButton
                aria-label="more"
                id="review-menu-button"
                aria-controls={open ? 'review-menu' : undefined}
                aria-expanded={open ? 'true' : undefined}
                aria-haspopup="true"
                onClick={handleClick}
              >
                <MoreVert />
              </IconButton>
              <Menu
                id="review-menu"
                MenuListProps={{
                  'aria-labelledby': 'review-menu-button',
                }}
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose}
              >
                {canEdit && (
                  <MenuItem onClick={handleEdit}>Edit</MenuItem>
                )}
                {canDelete && (
                  <MenuItem onClick={handleDelete}>Delete</MenuItem>
                )}
              </Menu>
            </>
          )}
        </Box>

        <Box display="flex" alignItems="center" mb={1}>
          <Rating value={review.rating} readOnly size="small" />
          <Typography variant="body2" color="text.secondary" sx={{ ml: 1 }}>
            {review.rating}/5
          </Typography>
        </Box>

        {review.comment && (
          <Typography variant="body1" color="text.primary">
            {review.comment}
          </Typography>
        )}

        {review.modifiedAt && review.modifiedAt !== review.createdAt && (
          <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: 'block' }}>
            Edited on {formatDate(review.modifiedAt)}
          </Typography>
        )}
      </CardContent>
    </Card>
  );
};