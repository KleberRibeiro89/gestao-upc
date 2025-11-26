import { BrowserRouter, Routes, Route, Outlet } from 'react-router-dom';
import { RouterTitle } from './RouterTitle';
import { commonsRoutes } from '../../pages/commons/routes';
import Template from '../template/Template';

const RouterApp = () => {
  return (
    <BrowserRouter>
      <RouterTitle />
      <Routes>
        <Route element={<Template><Outlet /></Template>}>
          {...commonsRoutes}
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default RouterApp;