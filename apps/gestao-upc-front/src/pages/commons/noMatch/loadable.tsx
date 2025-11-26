import { lazyLoad } from '@/shared/utils/loadable';

export const NoMatchPage = lazyLoad(
  () => import('.'),
  module => module.NoMatchPage,
);
