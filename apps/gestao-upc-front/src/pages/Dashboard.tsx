import { Button, Card, Typography, Space } from 'antd';
import { LogoutOutlined } from '@ant-design/icons';
import { useDispatch, useSelector } from 'react-redux';
import { logout, selectUser } from '../store/userSlice';

const { Title, Text } = Typography;

function Dashboard() {
  const dispatch = useDispatch();
  const user = useSelector(selectUser);

  const handleLogout = () => {
    dispatch(logout());
  };

  return (
    <div style={{
      minHeight: '100vh',
      background: '#f0f2f5',
      padding: '24px'
    }}>
      <Card
        style={{
          maxWidth: 1200,
          margin: '0 auto',
          boxShadow: '0 2px 8px rgba(0,0,0,0.1)'
        }}
      >
        <Space direction="vertical" size="large" style={{ width: '100%' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <div>
              <Title level={2} style={{ margin: 0 }}>
                Bem-vindo, {user.user}!
              </Title>
              <Text type="secondary">
                Sistema de Gestão UPC
              </Text>
            </div>
            <Button
              type="primary"
              danger
              icon={<LogoutOutlined />}
              onClick={handleLogout}
            >
              Sair
            </Button>
          </div>

          <Card>
            <Title level={4}>Dashboard</Title>
            <Text>Conteúdo do dashboard será exibido aqui.</Text>
          </Card>
        </Space>
      </Card>
    </div>
  );
}

export default Dashboard;

