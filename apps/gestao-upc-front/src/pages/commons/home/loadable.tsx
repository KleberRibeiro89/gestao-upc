import { lazyLoad } from '@/shared/utils/loadable';

export const HomePage = lazyLoad(
  () => import('.'),
  module => module.HomePage,
);
