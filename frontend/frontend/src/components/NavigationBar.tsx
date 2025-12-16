import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import { ArrowLeft, LogOut } from "lucide-react";
import { logout } from "../utils/auth";

export const NavigationBar = ({ isBackButton = false }) => {
  const navigate = useNavigate();

  const handleBack = () => {
    navigate(-1);
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <header className="flex justify-between items-center p-4 bg-[white]">
      {isBackButton ? (
        <Button variant="ghost" onClick={() => handleBack()}>
          <ArrowLeft />
        </Button>
      ) : (
        <div></div> // placeholder for spacing
      )}
      <Button variant="ghost" onClick={() => handleLogout()}>
        <LogOut />
      </Button>
    </header>
  );
};
