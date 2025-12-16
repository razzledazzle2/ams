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
import { AddAssetDialog } from "./Dialog";

const API_BASE = "http://localhost:5051";

type Asset = {
  id: string;
  name: string;
  category: string;
  value: number;
  createdAt: string;
};

export const Assets = () => {
  // const [showModal, setShowModal] = useState(false);
  const [assets, setAssets] = useState<Asset[]>([]);
  const [loading, setLoading] = useState(true);
  const fetchAssets = async () => {
    try {
      const res = await fetch(`${API_BASE}/api/assets`, {
        headers: {
          Authorization: `Bearer ${getAccessToken()}`,
        },
      });

      if (!res.ok) throw new Error("Failed to fetch assets");

      const data = await res.json();
      setAssets(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => {
    fetchAssets();
  }, []);

  return (
    <div>
      <NavigationBar isBackButton={false} title="Assets"></NavigationBar>
      <AddAssetDialog onAssetCreated={() => {}} />


    </div>
  );
};
