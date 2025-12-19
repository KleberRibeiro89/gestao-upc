import { useState } from 'react';
import { Form, Input, Button, Card, Typography, message } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { useDispatch } from 'react-redux';
import { changeUser } from '../store/userSlice';

const { Title } = Typography;

interface LoginValues {
  username: string;
  password: string;
}

function Login() {
  const [loading, setLoading] = useState(false);
  const dispatch = useDispatch();

  const onFinish = async (values: LoginValues) => {
    setLoading(true);
    try {
      // Simulação de autenticação - substitua pela sua lógica de API
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // Dispatch da ação de login
      dispatch(changeUser(values.username));
      message.success('Login realizado com sucesso!');
    } catch (error) {
      message.error('Erro ao fazer login. Verifique suas credenciais.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      minHeight: '100vh',
      background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
      padding: '20px'
    }}>
      <Card
        style={{
          width: '100%',
          maxWidth: 400,
          boxShadow: '0 8px 24px rgba(0,0,0,0.12)'
        }}
      >
        <div style={{ textAlign: 'center', marginBottom: 32 }}>
          <Title level={2} style={{ marginBottom: 8 }}>
            Gestão UPC
          </Title>
          <Typography.Text type="secondary">
            Faça login para continuar
          </Typography.Text>
        </div>

        <Form
          name="login"
          onFinish={onFinish}
          layout="vertical"
          requiredMark={false}
          size="large"
        >
          <Form.Item
            name="username"
            rules={[
              { required: true, message: 'Por favor, insira seu usuário!' }
            ]}
          >
            <Input
              prefix={<UserOutlined />}
              placeholder="Usuário"
            />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[
              { required: true, message: 'Por favor, insira sua senha!' }
            ]}
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="Senha"
            />
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              block
              style={{ height: 44 }}
            >
              Entrar
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
}

export default Login;

