import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import { ArrowLeft, LogOut } from "lucide-react";
import { logout } from "../utils/auth";

export const NavigationBar = ({ isBackButton = false, title}) => {
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
      <h1 className="text-lg font-medium">{title}</h1>
      <Button variant="ghost" onClick={() => handleLogout()}>
        <LogOut />
      </Button>
    </header>
  );
};
