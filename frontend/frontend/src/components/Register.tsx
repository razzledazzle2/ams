import { Box, Input, Button, Typography } from "@mui/material";
import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";

const API_BASE = "http://localhost:5051";

export const Register = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!username || !password || !confirmPassword) {
      setErrorMessage("All fields are required");
      return;
    }

    if (password !== confirmPassword) {
      setErrorMessage("Passwords do not match");
      return;
    }

    try {
      const response = await fetch(`${API_BASE}/api/users/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });

      const data = await response.json();

      if (!response.ok) {
        setErrorMessage(data || "Registration failed");
        return;
      }

      setErrorMessage("");
      setSuccessMessage("Account created. Redirecting to login…");
      setTimeout(() => navigate("/login"), 1500);
    } catch (err) {
      console.error(err);
      setErrorMessage("Something went wrong. Try again.");
    }
  };

  return (
    <Box
      sx={{
        width: "100vw",
        height: "100svh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        backgroundColor: "#20283E",
      }}
    >
      <Box
        sx={{
          width: "90%",
          maxWidth: "400px",
          backgroundColor: "#2D3B58",
          padding: "30px",
          borderRadius: "10px",
        }}
      >
        <Typography variant="h5" sx={{ color: "white", textAlign: "center", mb: 2 }}>
          Register
        </Typography>

        <form onSubmit={handleSubmit}>
          <Input
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            fullWidth
            sx={{ mb: 2, backgroundColor: "#1A2338", color: "white", p: 1 }}
          />

          <Input
            placeholder="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            fullWidth
            sx={{ mb: 2, backgroundColor: "#1A2338", color: "white", p: 1 }}
          />

          <Input
            placeholder="Confirm Password"
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            fullWidth
            sx={{ mb: 2, backgroundColor: "#1A2338", color: "white", p: 1 }}
          />

          {errorMessage && (
            <Typography sx={{ color: "red", mb: 1 }}>{errorMessage}</Typography>
          )}

          {successMessage && (
            <Typography sx={{ color: "green", mb: 1 }}>{successMessage}</Typography>
          )}

          <Button type="submit" variant="contained" fullWidth>
            Register
          </Button>

          <Typography sx={{ textAlign: "center", mt: 2, color: "white" }}>
            <Link to="/login" style={{ color: "#B0C4DE" }}>
              Already have an account? Login
            </Link>
          </Typography>
        </form>
      </Box>
    </Box>
  );
};
