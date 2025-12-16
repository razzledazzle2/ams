import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useState } from "react";
import { getAccessToken } from "@/utils/auth";
const API_BASE = "http://localhost:5051";

type Props = {
  onAssetCreated: () => void;
};
export function AddAssetDialog({ onAssetCreated }: Props) {
  const [name, setName] = useState("");
  const [category, setCategory] = useState("");
  const [value, setValue] = useState<number | "">("");
  const [loading, setLoading] = useState(false);

  const [open, setOpen] = useState(false);
  const close = () => {
    setOpen(false);
    setName("");
    setCategory("");
    setValue("");
  };
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const res = await fetch(`${API_BASE}/api/assets`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${getAccessToken()}`,
        },
        body: JSON.stringify({
          name,
          category,
          value,
        }),
      });

      if (!res.ok) throw new Error("Failed to create asset");

      onAssetCreated();
      close();
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button className="bg-blue-500 hover:bg-blue-300">Add Asset</Button>
      </DialogTrigger>

      <DialogContent className="sm:max-w-[425px]">
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>Create Asset</DialogTitle>
            <DialogDescription>
              Add a new asset to your portfolio.
            </DialogDescription>
          </DialogHeader>

          <div className="grid gap-4 py-4">
            <div className="grid gap-2">
              <Label htmlFor="name">Name</Label>
              <Input
                id="name"
                placeholder="e.g. Plates"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            </div>

            <div className="grid gap-2">
              <Label htmlFor="category">Category</Label>
              <Input
                id="category"
                placeholder="e.g. Kitchenware"
                value={category}
                onChange={(e) => setCategory(e.target.value)}
                required
              />
            </div>

            <div className="grid gap-2">
              <Label htmlFor="value">Value</Label>
              <Input
                id="value"
                type="number"
                placeholder="100"
                value={value}
                onChange={(e) =>
                  setValue(e.target.value === "" ? "" : Number(e.target.value))
                }
                required
              />
            </div>
          </div>

          <DialogFooter>
            <Button type="button" onClick={close} className="bg-red-500 hover:bg-red-900">
              Cancel
            </Button>

            <Button type="submit" className="bg-blue-500 hover:bg-blue-300">
              Create Asset
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
