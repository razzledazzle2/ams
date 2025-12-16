import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { getAccessToken } from "../utils/auth";
import { NavigationBar } from "./NavigationBar";

const API_BASE = "http://localhost:5051";

type Asset = {
  id: string;
  name: string;
  category: string;
  value: number;
  createdAt: string;
};

export const Assets = () => {
  return (
  <div>
    <NavigationBar
      isBackButton={false}
      title="Assets"
    ></NavigationBar>

  </div>
  );
};
