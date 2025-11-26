import { Route } from 'react-router';
import RouterPaths from '../../core/router/RouterPaths';
import { HomePage } from './home';
import { NoMatchPage } from './noMatch';


export const commonsRoutes = [
  <Route path={RouterPaths.HOME} element={<HomePage />} />,
  <Route path={RouterPaths.HOMEPAGE} element={<HomePage />} />,
  <Route path={RouterPaths.NOT_FOUND} element={<NoMatchPage />} />,

];