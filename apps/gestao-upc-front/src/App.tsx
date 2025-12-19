import { useSelector } from 'react-redux';
import { selectUser } from './store/userSlice';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';

function App() {
  const user = useSelector(selectUser);

  return (
    <>
      {user.isLogged ? <Dashboard /> : <Login />}
    </>
  )
}

export default App
