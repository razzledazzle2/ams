import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import { ArrowLeft, LogOut, Plus } from "lucide-react";
import { logout } from "../utils/auth";

export const NavigationBar = ({ onAdd, title }) => {
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <header className="flex justify-between items-center p-4 bg-[white]">
      <h1 className="text-lg font-medium">{title}</h1>
      <div>
        <Button variant="ghost" onClick={() => onAdd()}>
          <Plus />
        </Button>
        <Button variant="ghost" onClick={() => handleLogout()}>
          <LogOut />
        </Button>
      </div>
    </header>
  );
};
