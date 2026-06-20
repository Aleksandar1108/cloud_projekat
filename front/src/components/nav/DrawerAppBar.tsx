import * as React from 'react';

import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import Divider from '@mui/material/Divider';
import Drawer from '@mui/material/Drawer';
import IconButton from '@mui/material/IconButton';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import MenuIcon from '@mui/icons-material/Menu';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';

import { useNavigate } from 'react-router-dom';
import LoginButton from '../auth/LoginButton';

const drawerWidth = 240;


const navItems = [
    {
        label: "Home",
        path: "/"
    },
    {
        label: "Users",
        path: "/users"
    },
    {
        label: "Other",
        path: "/properties"
    }
];

export default function DrawerAppBar() {

    const [mobileOpen, setMobileOpen] =
        React.useState(false);

    const navigate = useNavigate();

    const handleDrawerToggle = () => {

        setMobileOpen((prevState) => !prevState);
    };

    const drawer = (

        <Box
            onClick={handleDrawerToggle}
            sx={{ textAlign: 'center' }}
        >

            <Typography
                variant="h6"
                sx={{ my: 2 }}
            >
                Smart Grid
            </Typography>

            <Divider />

            <List>

                {navItems.map((item) => (

                    <ListItem
                        key={item.label}
                        disablePadding
                    >

                        <ListItemButton
                            sx={{ textAlign: 'center' }}
                            onClick={() => navigate(item.path)}
                        >

                            <ListItemText
                                primary={item.label}
                            />

                        </ListItemButton>

                    </ListItem>
                ))}

            </List>

        </Box>
    );

    return (

        <Box sx={{ display: 'flex' }}>

            <CssBaseline />

            <AppBar component="nav">

                <Toolbar>

                    <IconButton
                        color="inherit"
                        aria-label="open drawer"
                        edge="start"
                        onClick={handleDrawerToggle}
                        sx={{
                            mr: 2,
                            display: { sm: 'none' }
                        }}
                    >
                        <MenuIcon />
                    </IconButton>

                    <Typography
                        variant="h6"
                        component="div"
                        sx={{
                            flexGrow: 1,
                            display: {
                                xs: 'none',
                                sm: 'block'
                            }
                        }}
                    >

                        <Button
                            onClick={() => navigate("/")}
                            sx={{
                                color: '#fff',
                                fontWeight: 700,
                                fontSize: "18px"
                            }}
                        >
                            Smart Grid
                        </Button>

                    </Typography>

                    <Box
                        sx={{
                            display: {
                                xs: 'none',
                                sm: 'block'
                            }
                        }}
                    >

                        {navItems.map((item) => (

                            <Button
                                key={item.label}
                                sx={{ color: '#fff' }}
                                onClick={() =>
                                    navigate(item.path)
                                }
                            >
                                {item.label}
                            </Button>
                        ))}

                    </Box>
                    <LoginButton />
                </Toolbar>

            </AppBar>

            <nav>

                <Drawer
                    variant="temporary"
                    open={mobileOpen}
                    onClose={handleDrawerToggle}
                    ModalProps={{
                        keepMounted: true,
                    }}
                    sx={{
                        display: {
                            xs: 'block',
                            sm: 'none'
                        },
                        '& .MuiDrawer-paper': {
                            boxSizing: 'border-box',
                            width: drawerWidth
                        },
                    }}
                >

                    {drawer}

                </Drawer>

            </nav>

            <Box
                component="main"
                sx={{ p: 3 }}
            >
                <Toolbar />
            </Box>

        </Box>
    );
}